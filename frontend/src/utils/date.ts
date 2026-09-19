/** Data de hoje (YYYY-MM-DD); se cair em sábado/domingo, avança para a próxima segunda-feira,
 * já que a grade letiva só cobre Segunda–Sexta. */
export function proximoDiaLetivo(): string {
  const data = new Date();
  const diaSemana = data.getDay(); // 0 = domingo, 6 = sábado
  if (diaSemana === 6) data.setDate(data.getDate() + 2);
  else if (diaSemana === 0) data.setDate(data.getDate() + 1);
  return data.toISOString().slice(0, 10);
}
