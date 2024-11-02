<script>
    document.addEventListener("DOMContentLoaded", function () {
        const addToCartButtons = document.querySelectorAll(".add-to-cart");

    // Función para actualizar el botón del carrito
    function updateCartButton() {
            const cart = JSON.parse(localStorage.getItem("cart")) || [];
            const cartCount = cart.reduce((total, product) => total + product.quantity, 0);

    const cartButton = document.getElementById("cart-button");
    const cartCountSpan = document.getElementById("cart-count");

    // Actualizar el contador de carrito y mostrar/ocultar el botón
    cartCountSpan.textContent = cartCount;
            cartButton.style.display = cartCount > 0 ? "inline-block" : "none";
        }

    // Inicializar el botón de carrito al cargar la página
    updateCartButton();

        // Manejar el evento de agregar al carrito
        addToCartButtons.forEach(button => {
        button.addEventListener("click", function (e) {
            e.preventDefault();

            const productId = this.getAttribute("data-id");
            const productName = this.closest(".card-body").querySelector(".card-title").innerText;
            const productPrice = parseFloat(this.closest(".card-body").querySelector(".card-text:nth-child(3)").innerText.replace("Price: ", ""));

            let cart = JSON.parse(localStorage.getItem("cart")) || [];

            const existingProduct = cart.find(p => p.id === productId);

            if (existingProduct) {
                existingProduct.quantity += 1;
            } else {
                const product = { id: productId, name: productName, price: productPrice, quantity: 1 };
                cart.push(product);
            }

            localStorage.setItem("cart", JSON.stringify(cart));
            alert("Product added to cart!");

            // Actualizar el botón del carrito
            updateCartButton();
        });
        });
    });

    // Redirigir a la vista CartView al hacer clic en el botón de carrito
    function goToCart() {
        window.location.href = '@Url.Action("CartView", "Home")';
    }
</script>

