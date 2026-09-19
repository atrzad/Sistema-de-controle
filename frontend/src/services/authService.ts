import { api } from './api';
import type { LoginResponse, Usuario } from '../types/auth';

export const authService = {
  async login(email: string, senha: string): Promise<LoginResponse> {
    const { data } = await api.post<LoginResponse>('/auth/login', { email, senha });
    return data;
  },

  async me(): Promise<Usuario> {
    const { data } = await api.get<Usuario>('/auth/me');
    return data;
  },
};
