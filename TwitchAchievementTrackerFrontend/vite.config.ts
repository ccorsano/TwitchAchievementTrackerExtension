import { defineConfig } from 'vite';
import { svelte } from '@sveltejs/vite-plugin-svelte';
import { resolve } from 'path';


export default defineConfig({
    base: './',
    build: {
        rollupOptions: {
            input: {
                video_overlay: resolve(__dirname, 'video_overlay.html'),
                config: resolve(__dirname, 'config.html'),
                live_config: resolve(__dirname, 'live_config.html'),
                mobile: resolve(__dirname, 'mobile.html')
            }
        },
        assetsDir: "."
    },
    server: {
        https: true,
        port: 8090,
    },
    plugins: [
        svelte()
    ]
});
