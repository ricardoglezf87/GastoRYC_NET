import { defineConfig } from 'vite';
import laravel from 'laravel-vite-plugin';
import postcssConfig from './postcss.config.cjs';

export default defineConfig({
    plugins: [
        laravel({
            input: [
                'resources/js/app.js',
                'resources/css/app.css',
                'resources/js/accounts_types/scripts.js',
                'resources/css/accounts_types/styles.css',
                //'resources/js/transactions/scripts.js',
                //'resources/css/transactions/styles.css',
            ],
            refresh: true,
        }),
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
    css: {
        plugins: postcssConfig.plugins || [],
      },
});
