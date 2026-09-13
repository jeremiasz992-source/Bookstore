import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { checkout } from "../api/orders";
import { clearCart, getCart, removeCartItem, setCartItemQuantity } from "../api/cart";
import { Message } from "../components/Message.jsx";
import { formatPrice } from "../labels.js";

export function CartPage() {
  const navigate = useNavigate();
  const [cart, setCart] = useState({ items: [] });
  const [message, setMessage] = useState({ error: "", success: "" });

  async function loadCart() {
    const data = await getCart();
    setCart(data);
  }

  useEffect(() => {
    loadCart().catch((err) => setMessage({ error: err.message, success: "" }));
  }, []);

  async function updateQuantity(bookId, quantity) {
    setMessage({ error: "", success: "" });
    try {
      await setCartItemQuantity(bookId, { bookId, quantity });
      await loadCart();
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  async function removeItem(bookId) {
    setMessage({ error: "", success: "" });
    try {
      await removeCartItem(bookId);
      await loadCart();
      setMessage({ error: "", success: "Usunięto pozycję z koszyka." });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  async function handleClearCart() {
    setMessage({ error: "", success: "" });
    try {
      await clearCart();
      await loadCart();
      setMessage({ error: "", success: "Wyczyszczono koszyk." });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  async function handleCheckout() {
    setMessage({ error: "", success: "" });
    try {
      const order = await checkout();
      await loadCart();
      navigate("/order-confirmation", { state: { order } });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  const total = cart.items.reduce((sum, item) => sum + item.price * item.quantity, 0);

  return (
    <section className="panel">
      <h1>Koszyk</h1>
      <Message error={message.error} success={message.success} />
      {!cart.items.length && <p>Koszyk jest pusty.</p>}
      <div className="list">
        {cart.items.map((item) => (
          <div className="row-item" key={item.bookId}>
            <div>
              <strong>{item.title}</strong>
              <p>{formatPrice(item.price)} PLN &times; {item.quantity} = {formatPrice(item.price * item.quantity)} PLN</p>
            </div>
            <input type="number" min="0" value={item.quantity} onChange={(e) => updateQuantity(item.bookId, Number(e.target.value))} />
            <button type="button" onClick={() => removeItem(item.bookId)}>Usuń</button>
          </div>
        ))}
      </div>
      <div className="actions">
        <strong>Suma: {formatPrice(total)} PLN</strong>
        <button type="button" disabled={!cart.items.length} onClick={handleCheckout}>Złóż zamówienie</button>
        <button type="button" disabled={!cart.items.length} onClick={handleClearCart}>Wyczyść</button>
      </div>
    </section>
  );
}
