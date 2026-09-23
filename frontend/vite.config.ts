import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // Em dev, repassa /api para a API local (mesmo comportamento do nginx em produção).
    proxy: {
      '/api': process.env.VITE_API_PROXY_TARGET ?? 'http://localhost:5000',
    },
  },
});
