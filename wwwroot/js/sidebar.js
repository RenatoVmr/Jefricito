document.addEventListener('DOMContentLoaded', function() {
    const toggleSidebarButton = document.getElementById('toggle-sidebar');
    const sidebar = document.querySelector('.sidebar');
    
    // Si estamos en móvil, agregar el botón de toggle si no existe
    if (window.innerWidth <= 768 && !toggleSidebarButton) {
        const button = document.createElement('button');
        button.id = 'toggle-sidebar';
        button.className = 'toggle-sidebar-btn';
        button.innerHTML = '<i class="bi bi-list"></i>';
        document.querySelector('.main-content').prepend(button);
        
        // Agregar los estilos del botón
        const style = document.createElement('style');
        style.textContent = `
            .toggle-sidebar-btn {
                position: fixed;
                top: 1rem;
                left: 1rem;
                z-index: 1001;
                background: var(--primary-color);
                border: none;
                color: white;
                width: 42px;
                height: 42px;
                border-radius: 21px;
                display: flex;
                align-items: center;
                justify-content: center;
                cursor: pointer;
                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                transition: background-color 0.3s;
            }
            
            .toggle-sidebar-btn:hover {
                background: #1557b0;
            }
            
            .toggle-sidebar-btn i {
                font-size: 1.5rem;
            }
            
            @media (min-width: 769px) {
                .toggle-sidebar-btn {
                    display: none;
                }
            }
        `;
        document.head.appendChild(style);
        
        // Agregar el evento click
        button.addEventListener('click', function() {
            sidebar.classList.toggle('show');
        });
    }

    // Cerrar el sidebar cuando se hace click en un enlace (en móvil)
    document.querySelectorAll('.sidebar .nav-link').forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth <= 768) {
                sidebar.classList.remove('show');
            }
        });
    });

    // Cerrar el sidebar cuando se hace click fuera de él (en móvil)
    document.addEventListener('click', (e) => {
        if (window.innerWidth <= 768 && 
            !sidebar.contains(e.target) && 
            e.target.id !== 'toggle-sidebar') {
            sidebar.classList.remove('show');
        }
    });
});