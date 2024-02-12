import { defineConfig } from 'vite';
import laravel from 'laravel-vite-plugin';
import Vue from '@vitejs/plugin-vue';

export default defineConfig({
    plugins: [
        laravel({
            input: [
                'resources/js/app.js',
                'resources/css/app.css',
                'resources/js/accounts_types/scripts.js',
                'resources/css/accounts_types/styles.css',
            ],
            refresh: true,
        }),
        Vue(),
    ],
    server: {
        proxy: {
            '/': 'http://localhost', // Apunta a tu servidor Laravel
        },
    },
    build: {
        minify: 'esbuild',
        emptyOutDir: true,
        outDir: 'public/build',
        assetsDir: '.',
    },
});

