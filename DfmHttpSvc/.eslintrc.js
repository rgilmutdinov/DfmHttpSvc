module.exports = {
    env: {
        browser: true,
        es6: true
    },
    globals: {
        "$": true,
        "jQuery": true
    },
    parserOptions: {
        parser: 'babel-eslint',
        sourceType: 'module',
        ecmaVersion: 6
    },
    extends: [
        // https://github.com/vuejs/eslint-plugin-vue#priority-a-essential-error-prevention
        // consider switching to `plugin:vue/strongly-recommended` or `plugin:vue/recommended` for stricter rules.
        'eslint:recommended',
        'plugin:vue/essential'
    ],
    rules: {
        // general eslint rules
        'brace-style': ['warn', '1tbs', { 'allowSingleLine': true }],
        'comma-dangle': 'warn',
        'comma-spacing': ['error', { 'before': false, 'after': true }],
        'comma-style': ['warn', 'last'],
        'eqeqeq': 'error',
        'no-console': 'warn',
        'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
        'no-extra-semi': 'warn',
        'no-irregular-whitespace': 'warn',
        'no-restricted-globals': 'warn',
        'no-trailing-spaces': "warn",
        'no-undef': 'warn',
        'no-unused-vars': 'warn',
        'semi': 'warn',
        'semi-spacing': 'warn',
        'valid-jsdoc': [
            'error',
            { 'requireReturn': false }
        ],

        // vue rules
        'vue/require-v-for-key': 'warn'
    },
    plugins: [
        'vue'
    ]
}