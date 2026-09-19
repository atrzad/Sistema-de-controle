export interface Usuario {
  id: number;
  nome: string;
  email: string;
}

export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  usuario: Usuario;
}
