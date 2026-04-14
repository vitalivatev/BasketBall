// Продукти, които ще се показват в магазина
const products = [
    { id: 1, name: 'Лаптоп', price: 1299.99, emoji: '💻' },
    { id: 2, name: 'Телефон', price: 899.99, emoji: '📱' },
    { id: 3, name: 'Слушалки', price: 199.99, emoji: '🎧' },
    { id: 4, name: 'Камера', price: 599.99, emoji: '📷' },
    { id: 5, name: 'Таблет', price: 449.99, emoji: '📱' },
    { id: 6, name: 'Часовник', price: 299.99, emoji: '⌚' },
    { id: 7, name: 'Гейминг конзола', price: 399.99, emoji: '🎮' },
    { id: 8, name: 'Клавиатура', price: 89.99, emoji: '⌨️' },
    { id: 9, name: 'Мишка', price: 49.99, emoji: '🖱️' },
    { id: 10, name: 'Монитор', price: 349.99, emoji: '🖥️' },
    { id: 11, name: 'Принтер', price: 179.99, emoji: '🖨️' },
    { id: 12, name: 'USB флашка', price: 19.99, emoji: '💾' }
];

// Количка за пазаруване
let cart = [];

// Инициализация на приложението
document.addEventListener('DOMContentLoaded', function() {
    loadProducts();
    updateCartDisplay();
    
    // Бутон за поръчка
    document.getElementById('checkout-btn').addEventListener('click', checkout);
});

// Зареждане на продуктите в магазина
function loadProducts() {
    const productGrid = document.getElementById('product-grid');
    
    products.forEach(product => {
        const productCard = document.createElement('div');
        productCard.className = 'product-card';
        productCard.innerHTML = `
            <div class="product-image">${product.emoji}</div>
            <div class="product-name">${product.name}</div>
            <div class="product-price">${product.price.toFixed(2)} лв.</div>
            <button class="add-to-cart-btn" onclick="addToCart(${product.id})">
                Добави в количката
            </button>
        `;
        productGrid.appendChild(productCard);
    });
}

// Добавяне на продукт в количката
function addToCart(productId) {
    const product = products.find(p => p.id === productId);
    if (!product) return;
    
    const existingItem = cart.find(item => item.id === productId);
    
    if (existingItem) {
        existingItem.quantity++;
    } else {
        cart.push({
            id: product.id,
            name: product.name,
            price: product.price,
            emoji: product.emoji,
            quantity: 1
        });
    }
    
    updateCartDisplay();
    showNotification(`${product.name} добавен в количката!`);
}

// Премахване на продукт от количката
function removeFromCart(productId) {
    cart = cart.filter(item => item.id !== productId);
    updateCartDisplay();
    showNotification('Продуктът е премахнат от количката');
}

// Обновяване на количеството на продукт
function updateQuantity(productId, change) {
    const item = cart.find(item => item.id === productId);
    if (!item) return;
    
    item.quantity += change;
    
    if (item.quantity <= 0) {
        removeFromCart(productId);
    } else {
        updateCartDisplay();
    }
}

// Обновяване на дисплея на количката
function updateCartDisplay() {
    const cartItems = document.getElementById('cart-items');
    const cartCount = document.getElementById('cart-count');
    const totalPrice = document.getElementById('total-price');
    const checkoutBtn = document.getElementById('checkout-btn');
    
    if (cart.length === 0) {
        cartItems.innerHTML = '<p class="empty-cart">Количката е празна</p>';
        cartCount.textContent = '0';
        totalPrice.textContent = '0.00';
        checkoutBtn.disabled = true;
        return;
    }
    
    let totalItems = 0;
    let total = 0;
    
    cartItems.innerHTML = cart.map(item => {
        totalItems += item.quantity;
        total += item.price * item.quantity;
        
        return `
            <div class="cart-item">
                <div class="cart-item-info">
                    <div class="cart-item-name">${item.emoji} ${item.name}</div>
                    <div class="cart-item-price">${item.price.toFixed(2)} лв. броя</div>
                </div>
                <div class="cart-item-quantity">
                    <button class="quantity-btn" onclick="updateQuantity(${item.id}, -1)">-</button>
                    <span class="quantity">${item.quantity}</span>
                    <button class="quantity-btn" onclick="updateQuantity(${item.id}, 1)">+</button>
                </div>
                <button class="remove-btn" onclick="removeFromCart(${item.id})">Премахни</button>
            </div>
        `;
    }).join('');
    
    cartCount.textContent = totalItems;
    totalPrice.textContent = total.toFixed(2);
    checkoutBtn.disabled = false;
}

// Поръчка
function checkout() {
    if (cart.length === 0) return;
    
    const total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const itemCount = cart.reduce((sum, item) => sum + item.quantity, 0);
    
    const confirmMessage = `Потвърдете поръчка:\n\n` +
        `Брой продукти: ${itemCount}\n` +
        `Обща сума: ${total.toFixed(2)} лв.\n\n` +
        `Желаете ли да потвърдите поръчката?`;
    
    if (confirm(confirmMessage)) {
        cart = [];
        updateCartDisplay();
        showNotification('Поръчката е потвърдена успешно! Благодарим Ви!');
    }
}

// Показване на известие
function showNotification(message) {
    // Създаване на елемент за известие
    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: linear-gradient(45deg, #27ae60, #2ecc71);
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
        z-index: 1000;
        font-weight: bold;
        animation: slideIn 0.3s ease;
    `;
    notification.textContent = message;
    
    // Добавяне на CSS анимация
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);
    
    document.body.appendChild(notification);
    
    // Премахване на известието след 3 секунди
    setTimeout(() => {
        notification.style.animation = 'slideIn 0.3s ease reverse';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}
