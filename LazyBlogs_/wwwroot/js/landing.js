// ═══════════════════════════════════════════════════
// landing.js — LazyBlogging_ (ASP.NET Core MVC v10)
// GSAP animations, Light/Dark mode, sticky panels
// ═══════════════════════════════════════════════════

(function () {
    'use strict';

    // ─── DOM refs ──────────────────────────────────
    const html = document.documentElement;
    const themeToggle = document.getElementById('themeToggle');
    const themeIcon = document.getElementById('themeIcon');
    const heroSection = document.querySelector('.lb-hero');
    const heroEyebrow = document.querySelector('.lb-hero__eyebrow');
    const heroHeadline = document.querySelector('.lb-hero__headline');
    const heroSub = document.querySelector('.lb-hero__sub');
    const heroCta = document.querySelector('.lb-hero__cta');
    const heroStats = document.querySelector('.lb-hero__stats');
    const heroScrollHint = document.querySelector('.lb-hero__scroll-hint');
    const stickyPanels = document.querySelectorAll('.lb-sticky-panel');
    const featureCards = document.querySelectorAll('.lb-feature-card');
    const roleCards = document.querySelectorAll('.lb-role-card');
    const statNums = document.querySelectorAll('.lb-stat__num');
    const marquee = document.querySelector('.lb-marquee');

    // ─── Check if GSAP is loaded ──────────────────
    function isGsapLoaded() {
        return typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined';
    }

    // ─── Theme toggle ─────────────────────────────
    function getPreferredTheme() {
        const stored = localStorage.getItem('lb-theme');
        if (stored) return stored;
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    function setTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem('lb-theme', theme);
        if (themeIcon) {
            themeIcon.className = theme === 'dark' ? 'fa-solid fa-sun' : 'fa-solid fa-moon';
        }
    }

    setTheme(getPreferredTheme());

    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            const current = html.getAttribute('data-theme');
            const next = current === 'dark' ? 'light' : 'dark';
            setTheme(next);
        });
    }

    // ─── GSAP Animations ──────────────────────────
    function initGsapAnimations() {
        if (!isGsapLoaded()) {
            console.warn('GSAP or ScrollTrigger not loaded. Falling back to CSS animations.');
            return fallbackAnimations();
        }

        // Register ScrollTrigger
        gsap.registerPlugin(ScrollTrigger);

        // ─── Hero Animations ──────────────────────
        const heroTimeline = gsap.timeline({
            defaults: { ease: 'power3.out', duration: 1.2 }
        });

        heroTimeline
            .fromTo(heroEyebrow,
                { opacity: 0, y: 30 },
                { opacity: 1, y: 0, duration: 0.8 }
            )
            .fromTo(heroHeadline,
                { opacity: 0, y: 40 },
                { opacity: 1, y: 0, duration: 1 },
                '-=0.4'
            )
            .fromTo(heroSub,
                { opacity: 0, y: 30 },
                { opacity: 1, y: 0, duration: 0.9 },
                '-=0.6'
            )
            .fromTo(heroCta,
                { opacity: 0, y: 30 },
                { opacity: 1, y: 0, duration: 0.8 },
                '-=0.5'
            )
            .fromTo(heroStats,
                { opacity: 0, y: 30 },
                { opacity: 1, y: 0, duration: 0.8 },
                '-=0.4'
            )
            .fromTo(heroScrollHint,
                { opacity: 0, y: 20 },
                { opacity: 0.6, y: 0, duration: 0.6 },
                '-=0.3'
            );

        // ─── Parallax effect on hero noise ────────
        const noise = document.querySelector('.lb-hero__noise');
        if (noise) {
            gsap.to(noise, {
                y: 100,
                ease: 'none',
                scrollTrigger: {
                    trigger: heroSection,
                    start: 'top top',
                    end: 'bottom top',
                    scrub: 1
                }
            });
        }

        // ─── Sticky Panels with GSAP ──────────────
        stickyPanels.forEach((panel, index) => {
            const stepNumber = panel.querySelector('.lb-step-number');
            const stepTitle = panel.querySelector('.lb-step-title');
            const stepBody = panel.querySelector('.lb-step-body');
            const stepList = panel.querySelector('.lb-step-list');
            const mock = panel.querySelector('.lb-mock');

            // Create a timeline for each panel
            const tl = gsap.timeline({
                scrollTrigger: {
                    trigger: panel,
                    start: 'top center',
                    end: 'bottom center',
                    toggleActions: 'play none none reverse',
                    onEnter: () => {
                        // Add active class when panel enters viewport
                        stickyPanels.forEach(p => p.classList.remove('is-active'));
                        panel.classList.add('is-active');
                    },
                    onEnterBack: () => {
                        stickyPanels.forEach(p => p.classList.remove('is-active'));
                        panel.classList.add('is-active');
                    }
                }
            });

            // Animate step number
            if (stepNumber) {
                tl.fromTo(stepNumber,
                    { scale: 0.8, opacity: 0.3 },
                    { scale: 1, opacity: 1, duration: 0.6, ease: 'back.out(1.7)' },
                    0
                );
            }

            // Animate text content
            if (stepTitle) {
                tl.fromTo(stepTitle,
                    { x: -30, opacity: 0 },
                    { x: 0, opacity: 1, duration: 0.7, ease: 'power2.out' },
                    0.1
                );
            }

            if (stepBody) {
                tl.fromTo(stepBody,
                    { x: -30, opacity: 0 },
                    { x: 0, opacity: 1, duration: 0.7, ease: 'power2.out' },
                    0.2
                );
            }

            if (stepList) {
                const items = stepList.querySelectorAll('li');
                items.forEach((item, i) => {
                    tl.fromTo(item,
                        { x: -20, opacity: 0 },
                        { x: 0, opacity: 1, duration: 0.5, ease: 'power2.out' },
                        0.3 + (i * 0.1)
                    );
                });
            }

            // Animate mock with slight rotation and scale
            if (mock) {
                tl.fromTo(mock,
                    { y: 40, opacity: 0, scale: 0.95, rotationX: 2 },
                    { y: 0, opacity: 1, scale: 1, rotationX: 0, duration: 0.9, ease: 'power2.out' },
                    0.1
                );
            }
        });

        // ─── Feature Cards Animation ──────────────
        featureCards.forEach((card, index) => {
            const icon = card.querySelector('.lb-feature-card__icon');
            const title = card.querySelector('h4');
            const desc = card.querySelector('p');

            gsap.fromTo(card,
                { y: 60, opacity: 0, scale: 0.95 },
                {
                    y: 0,
                    opacity: 1,
                    scale: 1,
                    duration: 0.8,
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: card,
                        start: 'top 85%',
                        end: 'top 60%',
                        toggleActions: 'play none none reverse'
                    }
                }
            );

            // Stagger icon animation
            if (icon) {
                gsap.fromTo(icon,
                    { scale: 0, rotation: -30 },
                    {
                        scale: 1,
                        rotation: 0,
                        duration: 0.6,
                        ease: 'back.out(2)',
                        scrollTrigger: {
                            trigger: card,
                            start: 'top 85%',
                            end: 'top 60%',
                            toggleActions: 'play none none reverse'
                        }
                    }
                );
            }
        });

        // ─── Role Cards Animation ──────────────────
        roleCards.forEach((card, index) => {
            const iconWrap = card.querySelector('.lb-role-card__icon-wrap');
            const listItems = card.querySelectorAll('.lb-role-card__list li');

            // Stagger the card entrance
            gsap.fromTo(card,
                { y: 80, opacity: 0 },
                {
                    y: 0,
                    opacity: 1,
                    duration: 1,
                    ease: 'power3.out',
                    scrollTrigger: {
                        trigger: card,
                        start: 'top 85%',
                        end: 'top 60%',
                        toggleActions: 'play none none reverse'
                    }
                }
            );

            // Animate icon with bounce
            if (iconWrap) {
                gsap.fromTo(iconWrap,
                    { scale: 0, rotation: -20 },
                    {
                        scale: 1,
                        rotation: 0,
                        duration: 0.8,
                        ease: 'back.out(2.5)',
                        scrollTrigger: {
                            trigger: card,
                            start: 'top 85%',
                            end: 'top 60%',
                            toggleActions: 'play none none reverse'
                        }
                    }
                );
            }

            // Stagger list items
            listItems.forEach((item, i) => {
                gsap.fromTo(item,
                    { x: -20, opacity: 0 },
                    {
                        x: 0,
                        opacity: 1,
                        duration: 0.5,
                        delay: i * 0.08,
                        ease: 'power2.out',
                        scrollTrigger: {
                            trigger: card,
                            start: 'top 85%',
                            end: 'top 60%',
                            toggleActions: 'play none none reverse'
                        }
                    }
                );
            });
        });

        // ─── Stats Counter Animation ──────────────
        statNums.forEach((stat) => {
            const text = stat.textContent.trim();
            let targetValue = 0;
            let suffix = '';

            if (text.includes('%')) {
                targetValue = parseFloat(text);
                suffix = '%';
            } else if (text.includes('roles')) {
                targetValue = 3;
            } else if (text.toLowerCase() === 'open') {
                // Just pulse effect
                gsap.fromTo(stat,
                    { color: 'var(--lb-accent)' },
                    {
                        color: 'var(--lb-text)',
                        duration: 2,
                        ease: 'power2.inOut',
                        scrollTrigger: {
                            trigger: stat,
                            start: 'top 80%',
                            end: 'top 60%',
                            toggleActions: 'play none none reverse'
                        }
                    }
                );
                return;
            } else {
                targetValue = parseInt(text) || 0;
            }

            if (targetValue > 0) {
                // Animate number counting up
                const obj = { value: 0 };
                gsap.to(obj, {
                    value: targetValue,
                    duration: 2,
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: stat,
                        start: 'top 80%',
                        end: 'top 60%',
                        toggleActions: 'play none none reverse'
                    },
                    onUpdate: function () {
                        if (suffix === '%') {
                            stat.textContent = obj.value.toFixed(0) + '%';
                        } else if (targetValue === 3) {
                            stat.textContent = Math.round(obj.value) + ' roles';
                        } else {
                            stat.textContent = Math.round(obj.value);
                        }
                    },
                    onComplete: function () {
                        if (suffix === '%') {
                            stat.textContent = targetValue + '%';
                        } else if (targetValue === 3) {
                            stat.textContent = '3 roles';
                        } else {
                            stat.textContent = targetValue;
                        }
                    }
                });
            }
        });

        // ─── Marquee Animation ─────────────────────
        if (marquee) {
            // Slow down marquee animation
            gsap.to(marquee, {
                x: '-50%',
                duration: 30,
                ease: 'none',
                repeat: -1,
                modifiers: {
                    x: (x) => parseFloat(x) % (marquee.scrollWidth / 2) + 'px'
                }
            });
        }

        // ─── Quote Cards Animation ──────────────────
        const quotes = document.querySelectorAll('.lb-quote');
        quotes.forEach((quote, index) => {
            gsap.fromTo(quote,
                { x: -30, opacity: 0 },
                {
                    x: 0,
                    opacity: 1,
                    duration: 0.8,
                    delay: index * 0.15,
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: quote,
                        start: 'top 85%',
                        end: 'top 60%',
                        toggleActions: 'play none none reverse'
                    }
                }
            );
        });

        // ─── Final CTA Animation ────────────────────
        const finalCta = document.querySelector('.lb-cta-final');
        if (finalCta) {
            const ctaTitle = finalCta.querySelector('.lb-cta-final__title');
            const ctaSub = finalCta.querySelector('.lb-cta-final__sub');
            const ctaBtn = finalCta.querySelector('.lb-btn');

            const ctaTimeline = gsap.timeline({
                scrollTrigger: {
                    trigger: finalCta,
                    start: 'top 80%',
                    end: 'top 50%',
                    toggleActions: 'play none none reverse'
                }
            });

            if (ctaTitle) {
                ctaTimeline.fromTo(ctaTitle,
                    { y: 40, opacity: 0, scale: 0.95 },
                    { y: 0, opacity: 1, scale: 1, duration: 0.9, ease: 'power3.out' }
                );
            }

            if (ctaSub) {
                ctaTimeline.fromTo(ctaSub,
                    { y: 30, opacity: 0 },
                    { y: 0, opacity: 1, duration: 0.7, ease: 'power2.out' },
                    '-=0.4'
                );
            }

            if (ctaBtn) {
                ctaTimeline.fromTo(ctaBtn,
                    { y: 30, opacity: 0, scale: 0.9 },
                    { y: 0, opacity: 1, scale: 1, duration: 0.6, ease: 'back.out(1.7)' },
                    '-=0.3'
                );
            }
        }

        // ─── Refresh ScrollTrigger on load ──────────
        ScrollTrigger.refresh();

        console.log('🚀 LazyBlogging_ GSAP animations initialized');
    }

    // ─── Fallback CSS Animations ──────────────────
    function fallbackAnimations() {
        console.warn('GSAP not available. Using CSS fallback animations.');

        // Simple intersection observer for hero
        const heroObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const elements = [heroEyebrow, heroHeadline, heroSub, heroCta, heroStats, heroScrollHint];
                    elements.forEach((el, index) => {
                        if (el) {
                            el.style.opacity = '0';
                            el.style.transform = 'translateY(20px)';
                            el.style.transition = `opacity 0.8s ease ${index * 0.15}s, transform 0.8s ease ${index * 0.15}s`;
                            void el.offsetHeight;
                            el.style.opacity = '1';
                            el.style.transform = 'translateY(0)';
                        }
                    });
                    heroObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });

        if (document.querySelector('.lb-hero__body')) {
            heroObserver.observe(document.querySelector('.lb-hero__body'));
        }

        // Sticky panels observer
        const panelObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    stickyPanels.forEach(p => p.classList.remove('is-active'));
                    entry.target.classList.add('is-active');
                }
            });
        }, { threshold: 0.4 });

        stickyPanels.forEach(panel => {
            panelObserver.observe(panel);
        });

        // Feature cards fade in
        const cardObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        }, { threshold: 0.1 });

        featureCards.forEach(card => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(30px)';
            card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
            cardObserver.observe(card);
        });
    }

    // ─── Smooth scroll for anchor links ────────────
    function setupSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                const targetId = this.getAttribute('href');
                if (targetId === '#') return;
                const target = document.querySelector(targetId);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    // ─── Hover Effects (GSAP enhanced) ────────────
    function setupHoverEffects() {
        // Feature card hover with GSAP
        featureCards.forEach(card => {
            card.addEventListener('mouseenter', function () {
                const icon = this.querySelector('.lb-feature-card__icon');
                if (icon && isGsapLoaded()) {
                    gsap.to(icon, {
                        scale: 1.2,
                        rotation: 10,
                        duration: 0.3,
                        ease: 'back.out(2)'
                    });
                } else if (icon) {
                    icon.style.transform = 'scale(1.2) rotate(10deg)';
                }
            });

            card.addEventListener('mouseleave', function () {
                const icon = this.querySelector('.lb-feature-card__icon');
                if (icon && isGsapLoaded()) {
                    gsap.to(icon, {
                        scale: 1,
                        rotation: 0,
                        duration: 0.3,
                        ease: 'power2.out'
                    });
                } else if (icon) {
                    icon.style.transform = 'scale(1) rotate(0deg)';
                }
            });
        });

        // Role card hover with GSAP
        roleCards.forEach(card => {
            card.addEventListener('mouseenter', function () {
                const iconWrap = this.querySelector('.lb-role-card__icon-wrap');
                if (iconWrap && isGsapLoaded()) {
                    gsap.to(iconWrap, {
                        scale: 1.1,
                        rotation: 5,
                        duration: 0.3,
                        ease: 'back.out(1.7)'
                    });
                } else if (iconWrap) {
                    iconWrap.style.transform = 'scale(1.1) rotate(5deg)';
                }
            });

            card.addEventListener('mouseleave', function () {
                const iconWrap = this.querySelector('.lb-role-card__icon-wrap');
                if (iconWrap && isGsapLoaded()) {
                    gsap.to(iconWrap, {
                        scale: 1,
                        rotation: 0,
                        duration: 0.3,
                        ease: 'power2.out'
                    });
                } else if (iconWrap) {
                    iconWrap.style.transform = 'scale(1) rotate(0deg)';
                }
            });
        });
    }

    // ─── Initialize ──────────────────────────────
    function init() {
        // Load GSAP from CDN if not already loaded
        if (typeof gsap === 'undefined') {
            console.log('Loading GSAP from CDN...');
            const script = document.createElement('script');
            script.src = 'https://cdnjs.cloudflare.com/ajax/libs/gsap/3.12.5/gsap.min.js';
            script.onload = function () {
                // Load ScrollTrigger after GSAP
                const stScript = document.createElement('script');
                stScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/gsap/3.12.5/ScrollTrigger.min.js';
                stScript.onload = function () {
                    console.log('GSAP and ScrollTrigger loaded successfully');
                    initGsapAnimations();
                    setupHoverEffects();
                };
                document.head.appendChild(stScript);
            };
            document.head.appendChild(script);
        } else {
            // GSAP already loaded
            initGsapAnimations();
            setupHoverEffects();
        }

        // Setup smooth scroll
        setupSmoothScroll();

        // Refresh ScrollTrigger on window resize
        let resizeTimer;
        window.addEventListener('resize', function () {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(() => {
                if (isGsapLoaded()) {
                    ScrollTrigger.refresh();
                }
            }, 250);
        });

        console.log('🚀 LazyBlogging_ landing initialized (ASP.NET Core MVC v10)');
    }

    // ─── DOM ready ─────────────────────────────────
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();