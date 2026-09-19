import { Fragment, useEffect, useState } from 'react';
import { frequenciaService } from '../../services/frequenciaService';
import { justificativaService } from '../../services/justificativaService';
import { extractErrorMessage } from '../../services/api';
import { usePersistedState } from '../../hooks/usePersistedState';
import { proximoDiaLetivo } from '../../utils/date';
import { STATUS_LABEL, type OcorrenciaDia } from '../../types/frequencia';

function JustificativaForm({ registroFrequenciaId, onConcluido }: { registroFrequenciaId: number; onConcluido: () => void }) {
  const [motivo, setMotivo] = useState('');
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    if (!arquivo) {
      setErro('Selecione o arquivo do atestado.');
      return;
    }
    setErro(null);
    setEnviando(true);
    try {
      await justificativaService.create(registroFrequenciaId, motivo, arquivo);
      onConcluido();
    } catch (error) {
      setErro(extractErrorMessage(error));
    } finally {
      setEnviando(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} style={{ marginTop: '0.6rem', display: 'flex', gap: '0.5rem', alignItems: 'center', flexWrap: 'wrap' }}>
      <input placeholder="Motivo da ausência" value={motivo} onChange={(e) => setMotivo(e.target.value)} required style={{ flex: 1, minWidth: 160, padding: '0.4rem' }} />
      <input type="file" accept=".pdf,.jpg,.jpeg,.png" onChange={(e) => setArquivo(e.target.files?.[0] ?? null)} required />
      <button className="btn btn--primary" type="submit" disabled={enviando}>Enviar atestado</button>
      {erro && <span className="error-message">{erro}</span>}
    </form>
  );
}

export function FrequenciaPage() {
  const [data, setData] = usePersistedState('frequencia:data', proximoDiaLetivo());
  const [ocorrencias, setOcorrencias] = useState<OcorrenciaDia[]>([]);
  const [erro, setErro] = useState<string | null>(null);
  const [justificandoAulaId, setJustificandoAulaId] = useState<number | null>(null);

  async function carregar() {
    try {
      setOcorrencias(await frequenciaService.getOcorrenciasDoDia(data));
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  useEffect(() => {
    carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [data]);

  async function marcar(ocorrencia: OcorrenciaDia, status: 'Presente' | 'Ausente') {
    setErro(null);
    try {
      if (ocorrencia.registroFrequenciaId) {
        await frequenciaService.atualizar(ocorrencia.registroFrequenciaId, status);
      } else {
        await frequenciaService.registrar(ocorrencia.aulaAgendadaId, data, status);
      }
      await carregar();
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  return (
    <div>
      <h1>Frequência do dia</h1>

      <div className="toolbar">
        <label>
          Data:{' '}
          <input type="date" value={data} onChange={(e) => setData(e.target.value)} />
        </label>
      </div>

      {erro && <p className="error-message">{erro}</p>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Horário</th>
              <th>Professor</th>
              <th>Turma</th>
              <th>Sala</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {ocorrencias.map((o) => (
              <Fragment key={o.aulaAgendadaId}>
                <tr>
                  <td>{o.horaInicio.slice(0, 5)} – {o.horaFim.slice(0, 5)}</td>
                  <td>{o.professorNome}</td>
                  <td>{o.turmaNome}</td>
                  <td>{o.salaNome}</td>
                  <td>
                    {o.status ? (
                      <span className={`badge ${o.status === 'Presente' ? 'badge--success' : 'badge--danger'}`}>
                        {STATUS_LABEL[o.status]}
                      </span>
                    ) : (
                      <span className="badge badge--muted">Não registrado</span>
                    )}
                  </td>
                  <td style={{ display: 'flex', gap: '0.4rem' }}>
                    <button className="btn" onClick={() => marcar(o, 'Presente')}>Presente</button>
                    <button className="btn btn--danger" onClick={() => marcar(o, 'Ausente')}>Ausente</button>
                    {o.registroFrequenciaId && o.status !== 'Presente' && (
                      <button className="btn" onClick={() => setJustificandoAulaId(o.aulaAgendadaId === justificandoAulaId ? null : o.aulaAgendadaId)}>
                        Justificar
                      </button>
                    )}
                  </td>
                </tr>
                {justificandoAulaId === o.aulaAgendadaId && o.registroFrequenciaId && (
                  <tr>
                    <td colSpan={6}>
                      <JustificativaForm
                        registroFrequenciaId={o.registroFrequenciaId}
                        onConcluido={() => {
                          setJustificandoAulaId(null);
                          carregar();
                        }}
                      />
                    </td>
                  </tr>
                )}
              </Fragment>
            ))}
            {ocorrencias.length === 0 && (
              <tr>
                <td colSpan={6}>Nenhuma aula prevista para esta data.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
