import { useEffect } from "react";
import { formatPrice } from "../labels.js";

const fallbackCover = "/covers/default-book.svg";

export function getBookCover(book) {
  return book?.coverImageUrl?.trim() || fallbackCover;
}

export function BookDetailsModal({ book, isAuthenticated, onAddToCart, onClose }) {
  useEffect(() => {
    if (!book) return undefined;

    function handleKeyDown(event) {
      if (event.key === "Escape") {
        onClose();
      }
    }

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [book, onClose]);

  if (!book) return null;

  return (
    <div className="modal-backdrop" role="presentation" onClick={onClose}>
      <div className="modal-card" role="dialog" aria-modal="true" aria-labelledby="book-details-title" onClick={(event) => event.stopPropagation()}>
        <img className="modal-cover" src={getBookCover(book)} alt={`Okładka książki ${book.title}`} />
        <div className="modal-content">
          <h2 id="book-details-title">{book.title}</h2>
          <dl className="book-details">
            <div><dt>Autor</dt><dd>{book.author}</dd></div>
            <div><dt>Wydawnictwo</dt><dd>{book.publisher}</dd></div>
            <div><dt>Rok wydania</dt><dd>{book.year}</dd></div>
            <div><dt>Kategoria</dt><dd>{book.categoryName}</dd></div>
            <div><dt>Cena</dt><dd>{formatPrice(book.price)} PLN</dd></div>
            <div><dt>Stan magazynowy</dt><dd>{book.stock}</dd></div>
          </dl>
          <p className="book-description">{book.description || "Brak opisu książki."}</p>
          <div className="modal-actions">
            <button type="button" disabled={!isAuthenticated || book.stock <= 0} onClick={() => onAddToCart(book.bookId)}>
              Dodaj do koszyka
            </button>
            <button type="button" className="secondary" onClick={onClose}>
              Zamknij
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
