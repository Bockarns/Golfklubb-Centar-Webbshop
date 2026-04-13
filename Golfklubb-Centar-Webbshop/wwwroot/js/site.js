

//Script för notifications badge counter
async function updateNotificationBadge() {
    try {
        const response = await fetch('/Notification/UnreadCounter');
        const count = await response.json();
        const badge = document.getElementById('notification-badge');

        if (badge) {
            if (count > 0) {
                badge.textContent = count > 99 ? '99+' : count;
                badge.style.display = 'block';
            } else {
                badge.style.display = 'none';
            }
        }
    } catch (error) {
        console.error('Kunde inte hämta notifikationer:', error);
    }
}

// Kör bara om användaren är inloggad (badge-elementet finns)
if (document.getElementById('notification-badge')) {
    updateNotificationBadge();
    setInterval(updateNotificationBadge, 30000);
}