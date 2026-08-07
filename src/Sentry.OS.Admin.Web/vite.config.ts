import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from 'node:path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    // Must match VITE_OIDC_REDIRECT_URI's port — an OIDC redirect URI has to match exactly, and
    // strictPort prevents Vite from silently falling back to another port and breaking the callback.
    port: 5174,
    strictPort: true,
  },
})
