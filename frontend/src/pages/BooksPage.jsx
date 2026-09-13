import { useEffect, useState } from "react";
import { getBooks } from "../api/books";
import { getCategories } from "../api/categories";
import { addCartItem } from "../api/cart";
import { BookDetailsModal, getBookCover } from "../components/BookDetailsModal.jsx";
import { Message } from "../components/Message.jsx";
import { useAuth } from "../context/AuthContext.jsx";
import { formatPrice } from "../labels.js";

// Liczba kart na stronie.
const PAGE_SIZE = 12;

export function BooksPage() {
  const { user } = useAuth();
  const [books, setBooks] = useState({ items: [], page: 1, pageSize: PAGE_SIZE, totalPages: 0, totalCount: 0 });
  const [categories, setCategories] = useState([]);
  const [categoryId, setCategoryId] = useState("");
  const [sort, setSort] = useState("");
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [message, setMessage] = useState({ error: "", success: "" });
  const [selectedBook, setSelectedBook] = useState(null);

  async function loadBooks() {
    const data = await getBooks({ page, pageSize: PAGE_SIZE, categoryId, sort, search });
    if (Array.isArray(data)) {
      setBooks({ items: data, page: 1, pageSize: data.length, totalPages: 1, totalCount: data.length });
      return;
    }

    setBooks({
      items: data?.items || [],
      page: data?.page || page,
      pageSize: data?.pageSize || PAGE_SIZE,
      totalPages: data?.totalPages || 0,
      totalCount: data?.totalCount || 0
    });
  }

  useEffect(() => {
    getCategories().then(setCategories).catch((err) => setMessage({ error: err.message, success: "" }));
  }, []);

  useEffect(() => {
    loadBooks().catch((err) => setMessage({ error: err.message, success: "" }));
  }, [page, categoryId, sort, search]);

  function handleSearch(event) {
    event.preventDefault();
    setSearch(searchInput.trim());
    setPage(1);
  }

  async function handleAdd(bookId) {
    setMessage({ error: "", success: "" });
    try {
      await addCartItem({ bookId, quantity: 1 });
      setMessage({ error: "", success: "Dodano książkę do koszyka." });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  return (
    <section>
      <div className="page-heading">
        <h1>Książki</h1>
        <form className="search-form" onSubmit={handleSearch}>
          <input
            type="search"
            placeholder="Szukaj po tytule lub autorze"
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
          />
          <button type="submit">Szukaj</button>
          {search && (
            <button type="button" className="secondary" onClick={() => { setSearchInput(""); setSearch(""); setPage(1); }}>
              Wyczyść
            </button>
          )}
        </form>
        <select value={categoryId} onChange={(e) => { setCategoryId(e.target.value); setPage(1); }}>
          <option value="">Wszystkie kategorie</option>
          {categories.map((category) => (
            <option key={category.categoryId} value={category.categoryId}>{category.name}</option>
          ))}
        </select>
        <select value={sort} onChange={(e) => { setSort(e.target.value); setPage(1); }}>
          <option value="">Sortuj: domyślnie</option>
          <option value="title">Tytuł (A-Z)</option>
          <option value="price_asc">Cena rosnąco</option>
          <option value="price_desc">Cena malejąco</option>
        </select>
      </div>
      <Message error={message.error} success={message.success} />
      <div className="grid">
        {books.items.map((book) => (
          <article className="card" key={book.bookId}>
            <img className="book-cover" src={getBookCover(book)} alt={`Okładka książki ${book.title}`} />
            <div className="book-card-body">
              <h2>{book.title}</h2>
              <p>{book.author}</p>
              <p>{book.categoryName} | {book.year}</p>
              <strong>{formatPrice(book.price)} PLN</strong>
              <p>Stan: {book.stock}</p>
              <div className="card-actions">
                <button type="button" disabled={!user || book.stock <= 0} onClick={() => handleAdd(book.bookId)}>
                  Dodaj do koszyka
                </button>
                <button type="button" className="secondary" onClick={() => setSelectedBook(book)}>
                  Szczegóły
                </button>
              </div>
            </div>
          </article>
        ))}
      </div>
      <div className="pager">
        <button type="button" disabled={page <= 1} onClick={() => setPage(page - 1)}>Poprzednia</button>
        <span>Strona {books.page} z {books.totalPages || 1} ({books.totalCount})</span>
        <button type="button" disabled={page >= books.totalPages} onClick={() => setPage(page + 1)}>Następna</button>
      </div>
      <BookDetailsModal
        book={selectedBook}
        isAuthenticated={Boolean(user)}
        onAddToCart={handleAdd}
        onClose={() => setSelectedBook(null)}
      />
    </section>
  );
}
