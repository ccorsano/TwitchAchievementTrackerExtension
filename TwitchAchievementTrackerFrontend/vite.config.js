import { defineConfig } from 'vite';
import { svelte } from '@sveltejs/vite-plugin-svelte';
import { sveltePreprocess } from 'svelte-preprocess';
import { resolve } from 'path';
import basicSsl from '@vitejs/plugin-basic-ssl';

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
        sourcemap: true,
        assetsDir: "."
    },
    server: {
        https: true,
        port: 8090,
    },
    plugins: [
        svelte({
            preprocess: sveltePreprocess()
        }),
        basicSsl({
            name: "localhost",
            domains: [
                "localhost"
            ],
        })
    ]
});
