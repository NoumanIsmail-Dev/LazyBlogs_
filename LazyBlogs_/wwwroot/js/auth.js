// ═══════════════════════════════════════════════════
// auth.js — LazyBlogging_ Login & Register
// ═══════════════════════════════════════════════════

(function () {
    'use strict';

    // ─── Password Toggle ──────────────────────────
    window.togglePasswordVisibility = function (button) {
        const wrapper = button.closest('.auth-password-wrapper');
        const input = wrapper.querySelector('.auth-input');
        const icon = button.querySelector('i');

        if (input.type === 'password') {
            input.type = 'text';
            icon.className = 'fa-solid fa-eye-slash';
        } else {
            input.type = 'password';
            icon.className = 'fa-solid fa-eye';
        }
    };

    // ─── Theme Toggle ─────────────────────────────
    function getPreferredTheme() {
        const stored = localStorage.getItem('lb-theme');
        if (stored) return stored;
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('lb-theme', theme);
    }

    // Initialize theme
    setTheme(getPreferredTheme());

    // ─── GSAP Animations ──────────────────────────
    function initAnimations() {
        if (typeof gsap === 'undefined') return;

        const card = document.querySelector('.auth-card');
        if (!card) return;

        // Animate card entrance
        gsap.fromTo(card,
            {
                opacity: 0,
                y: 40,
                scale: 0.95,
                rotationX: 5
            },
            {
                opacity: 1,
                y: 0,
                scale: 1,
                rotationX: 0,
                duration: 1,
                ease: 'power3.out'
            }
        );

        // Animate logo
        const logo = document.querySelector('.auth-logo');
        if (logo) {
            gsap.fromTo(logo,
                { opacity: 0, y: -20 },
                { opacity: 1, y: 0, duration: 0.8, delay: 0.2, ease: 'power2.out' }
            );
        }

        // Animate subtitle
        const subtitle = document.querySelector('.auth-subtitle');
        if (subtitle) {
            gsap.fromTo(subtitle,
                { opacity: 0, y: -10 },
                { opacity: 1, y: 0, duration: 0.6, delay: 0.3, ease: 'power2.out' }
            );
        }

        // Animate form elements with stagger
        const formGroups = document.querySelectorAll('.auth-form-group');
        const buttons = document.querySelectorAll('.auth-btn');
        const divider = document.querySelector('.auth-divider');

        const elements = [...formGroups];
        if (divider) elements.push(divider);
        if (buttons.length) elements.push(...buttons);

        elements.forEach((el, index) => {
            gsap.fromTo(el,
                { opacity: 0, y: 20 },
                {
                    opacity: 1,
                    y: 0,
                    duration: 0.6,
                    delay: 0.4 + (index * 0.08),
                    ease: 'power2.out'
                }
            );
        });

        // Animate footer
        const footer = document.querySelector('.auth-footer');
        if (footer) {
            gsap.fromTo(footer,
                { opacity: 0 },
                { opacity: 1, duration: 0.6, delay: 0.8, ease: 'power2.out' }
            );
        }
    }

    // ─── Form Validation Enhancements ─────────────
    function setupFormValidation() {
        const form = document.querySelector('.auth-form');
        if (!form) return;

        const inputs = form.querySelectorAll('.auth-input');
        inputs.forEach(input => {
            // Real-time validation
            input.addEventListener('blur', function () {
                validateField(this);
            });

            input.addEventListener('input', function () {
                // Clear error on typing
                const error = this.closest('.auth-form-group').querySelector('.auth-error');
                if (error && error.textContent) {
                    // Only clear if it was a validation error (not server error)
                    const isRequired = this.hasAttribute('required');
                    const hasValue = this.value.trim().length > 0;
                    if (isRequired && hasValue) {
                        error.textContent = '';
                    }
                }
            });
        });

        // Password confirmation validation
        const password = document.getElementById('Password');
        const confirmPassword = document.getElementById('ConfirmPassword');
        if (password && confirmPassword) {
            confirmPassword.addEventListener('input', function () {
                const error = this.closest('.auth-form-group').querySelector('.auth-error');
                if (password.value !== this.value) {
                    error.textContent = 'Passwords do not match';
                } else {
                    error.textContent = '';
                }
            });
        }
    }

    function validateField(input) {
        const error = input.closest('.auth-form-group').querySelector('.auth-error');
        if (!error) return;

        if (input.hasAttribute('required') && !input.value.trim()) {
            error.textContent = 'This field is required';
            return;
        }

        if (input.type === 'email' && input.value.trim()) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(input.value.trim())) {
                error.textContent = 'Please enter a valid email address';
                return;
            }
        }

        // Clear error if valid
        if (error.textContent) {
            error.textContent = '';
        }
    }

    // ─── Initialize ──────────────────────────────
    function init() {
        // Load GSAP if not available
        if (typeof gsap === 'undefined') {
            const script = document.createElement('script');
            script.src = 'https://cdnjs.cloudflare.com/ajax/libs/gsap/3.12.5/gsap.min.js';
            script.onload = function () {
                initAnimations();
            };
            document.head.appendChild(script);
        } else {
            initAnimations();
        }

        setupFormValidation();

        console.log('🚀 LazyBlogging_ auth page initialized');
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();