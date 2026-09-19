import { useEffect, useState } from 'react';

const PREFIX = 'sistema-controle:';

function ler<T>(chave: string, valorInicial: T): T {
  try {
    const bruto = localStorage.getItem(PREFIX + chave);
    return bruto !== null ? (JSON.parse(bruto) as T) : valorInicial;
  } catch {
    return valorInicial;
  }
}

/** Estado de componente que persiste em localStorage, sobrevivendo a reload da página. */
export function usePersistedState<T>(chave: string, valorInicial: T) {
  const [valor, setValor] = useState<T>(() => ler(chave, valorInicial));

  useEffect(() => {
    try {
      localStorage.setItem(PREFIX + chave, JSON.stringify(valor));
    } catch {
      // localStorage indisponível (ex: modo privado) — ignora, apenas não persiste
    }
  }, [chave, valor]);

  return [valor, setValor] as const;
}
