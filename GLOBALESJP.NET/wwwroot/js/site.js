/**
 * GLOBALES JP | Servicios TI & Software
 * Client-side interactive scripts
 */

document.addEventListener('DOMContentLoaded', function () {
    // 0. Theme Toggle (Dark / Light Mode)
    const themeToggleBtn = document.getElementById('themeToggleBtn');
    const themeIcon = document.getElementById('themeIcon');

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('jp-theme', theme);

        if (themeIcon) {
            if (theme === 'light') {
                themeIcon.className = 'bi bi-moon-stars-fill text-primary';
                if (themeToggleBtn) themeToggleBtn.setAttribute('title', 'Cambiar a Modo Oscuro');
            } else {
                themeIcon.className = 'bi bi-sun-fill text-warning';
                if (themeToggleBtn) themeToggleBtn.setAttribute('title', 'Cambiar a Modo Claro');
            }
        }
    }

    const savedTheme = localStorage.getItem('jp-theme') || 'dark';
    applyTheme(savedTheme);

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener('click', function () {
            const currentTheme = document.documentElement.getAttribute('data-bs-theme') || 'dark';
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            applyTheme(newTheme);
        });
    }

    // 1. Navbar dynamic styling on scroll
    const navbar = document.querySelector('.jp-navbar');
    const handleScroll = () => {
        if (!navbar) return;
        if (window.scrollY > 40) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
    };
    window.addEventListener('scroll', handleScroll, { passive: true });
    handleScroll();

    // 2. Quick pre-fill contact form from service cards
    window.selectServiceAndScroll = function (serviceName) {
        const serviceSelect = document.getElementById('contactServiceSelect');
        const contactSection = document.getElementById('contacto');
        const messageInput = document.getElementById('contactMessage');

        if (serviceSelect) {
            // Find option matching service
            for (let i = 0; i < serviceSelect.options.length; i++) {
                if (serviceSelect.options[i].value === serviceName || 
                    serviceSelect.options[i].text.toLowerCase().includes(serviceName.toLowerCase())) {
                    serviceSelect.selectedIndex = i;
                    break;
                }
            }
        }

        if (contactSection) {
            contactSection.scrollIntoView({ behavior: 'smooth' });
        }

        if (messageInput && !messageInput.value) {
            setTimeout(() => {
                messageInput.focus();
            }, 600);
        }
    };

    // 3. Send to WhatsApp directly from Form fields
    const btnSendWhatsApp = document.getElementById('btnSendWhatsApp');
    if (btnSendWhatsApp) {
        btnSendWhatsApp.addEventListener('click', function (e) {
            e.preventDefault();

            const name = document.getElementById('contactFullName')?.value.trim() || 'Cliente';
            const email = document.getElementById('contactEmail')?.value.trim() || 'No especificado';
            const phone = document.getElementById('contactPhone')?.value.trim() || 'No especificado';
            const company = document.getElementById('contactCompany')?.value.trim() || 'Particular';
            const service = document.getElementById('contactServiceSelect')?.value || 'Servicios TI Generales';
            const msg = document.getElementById('contactMessage')?.value.trim() || 'Deseo solicitar información y cotización sobre sus servicios.';

            const waText = 
                `*SOLICITUD DE SERVICIOS TI - GLOBALES JP*%0A` +
                `----------------------------------------%0A` +
                `*Nombre:* ${encodeURIComponent(name)}%0A` +
                `*Empresa:* ${encodeURIComponent(company)}%0A` +
                `*Correo:* ${encodeURIComponent(email)}%0A` +
                `*Teléfono:* ${encodeURIComponent(phone)}%0A` +
                `*Servicio:* ${encodeURIComponent(service)}%0A` +
                `----------------------------------------%0A` +
                `*Mensaje:*%0A${encodeURIComponent(msg)}%0A` +
                `----------------------------------------%0A` +
                `_Enviado desde el portal web corporativo_`;

            const waUrl = `https://wa.me/584142241795?text=${waText}`;
            window.open(waUrl, '_blank', 'noopener,noreferrer');
        });
    }

    // 4. Auto fade-out alert messages after 8 seconds
    const alertSuccess = document.querySelector('.alert-success');
    if (alertSuccess) {
        setTimeout(() => {
            alertSuccess.style.transition = 'opacity 0.6s ease';
            alertSuccess.style.opacity = '0';
            setTimeout(() => alertSuccess.remove(), 600);
        }, 8000);
    }
});
