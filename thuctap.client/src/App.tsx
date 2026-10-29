import { useEffect, useRef, useState, type FormEvent } from 'react';
import { addReview, createProduct, deleteProduct, getProduct, getProducts, updateProduct } from './api';
import type { Product, ProductInput, ProductSummary } from './types';
import ProductForm from './ProductForm';
import ProductList from './ProductList';
import './App.css';

const currency = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 });

function App() {
  const [products, setProducts] = useState<ProductSummary[]>([]);
  const [search, setSearch] = useState('');
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [isProductMenuOpen, setIsProductMenuOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoadingDetail, setIsLoadingDetail] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [detailError, setDetailError] = useState<string | null>(null);
  const [reviewAuthor, setReviewAuthor] = useState('');
  const [reviewRating, setReviewRating] = useState('5');
  const [reviewComment, setReviewComment] = useState('');
  const detailRequest = useRef<AbortController | null>(null);
  const productMenuTrigger = useRef<HTMLButtonElement>(null);
  const currentSearch = useRef(search);
  const activeProductId = useRef<string | null>(null);
  const detailGeneration = useRef(0);
  const mutationInFlight = useRef(false);

  useEffect(() => {
    const controller = new AbortController();
    currentSearch.current = search;
    getProducts(search, controller.signal)
      .then((items) => {
        if (!controller.signal.aborted) { setProducts(items); setError(null); }
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) setError(cause instanceof Error ? cause.message : 'Đã xảy ra lỗi khi tải sản phẩm.');
      })
      .finally(() => { if (!controller.signal.aborted) setIsLoading(false); });
    return () => controller.abort();
  }, [search]);

  async function refreshProducts() {
    const requestedSearch = currentSearch.current;
    try {
      const items = await getProducts(requestedSearch);
      if (currentSearch.current === requestedSearch) {
        setProducts(items);
        setError(null);
      }
    } catch (cause) {
      if (currentSearch.current === requestedSearch)
        setError(cause instanceof Error ? cause.message : 'Không thể làm mới danh sách sản phẩm.');
    }
  }

  async function openProduct(productId: string) {
    if (mutationInFlight.current) return;
    setIsProductMenuOpen(false);
    detailRequest.current?.abort();
    const controller = new AbortController();
    detailRequest.current = controller;
    detailGeneration.current += 1;
    const generation = detailGeneration.current;
    activeProductId.current = productId;
    setSelectedProduct(null);
    setIsLoadingDetail(true);
    setDetailError(null);
    setError(null);
    try {
      const product = await getProduct(productId, controller.signal);
      if (!controller.signal.aborted && detailGeneration.current === generation) setSelectedProduct(product);
    } catch (cause) {
      if (!controller.signal.aborted && detailGeneration.current === generation) setDetailError(cause instanceof Error ? cause.message : 'Không thể tải chi tiết sản phẩm.');
    } finally {
      if (!controller.signal.aborted && detailGeneration.current === generation) setIsLoadingDetail(false);
    }
  }

  function closeProductDetail() {
    setIsProductMenuOpen(false);
    detailRequest.current?.abort();
    detailGeneration.current += 1;
    activeProductId.current = null;
    setSelectedProduct(null);
  }

  async function saveProduct(input: ProductInput) {
    if (mutationInFlight.current) return;
    mutationInFlight.current = true;
    setIsSaving(true);
    setDetailError(null);
    try {
      const saved = selectedProduct ? await updateProduct(selectedProduct.id, input) : await createProduct(input);
      setSelectedProduct(saved);
      setIsCreating(false);
      setIsEditing(false);
      await refreshProducts();
    } catch (cause) {
      setDetailError(cause instanceof Error ? cause.message : 'Không thể lưu sản phẩm.');
    } finally {
      mutationInFlight.current = false;
      setIsSaving(false);
    }
  }

  async function removeProduct() {
    if (!selectedProduct || mutationInFlight.current || !window.confirm(`Xóa “${selectedProduct.name}”?`)) return;
    mutationInFlight.current = true;
    setIsSaving(true);
    try {
      await deleteProduct(selectedProduct.id);
      detailRequest.current?.abort();
      detailGeneration.current += 1;
      activeProductId.current = null;
      setSelectedProduct(null);
      setError(null);
      await refreshProducts();
    } catch (cause) {
      setDetailError(cause instanceof Error ? cause.message : 'Không thể xóa sản phẩm.');
    } finally {
      mutationInFlight.current = false;
      setIsSaving(false);
    }
  }

  async function submitReview(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!selectedProduct || mutationInFlight.current) return;
    const productId = selectedProduct.id;
    const generation = detailGeneration.current;
    mutationInFlight.current = true;
    setIsSaving(true);
    setDetailError(null);
    let reviewCreated = false;
    try {
      await addReview(productId, { author: reviewAuthor, rating: Number(reviewRating), comment: reviewComment });
      reviewCreated = true;
      if (activeProductId.current === productId && detailGeneration.current === generation) {
        setReviewAuthor('');
        setReviewComment('');
      }
      if (activeProductId.current === productId && detailGeneration.current === generation) {
        const refreshed = await getProduct(productId);
        if (activeProductId.current === productId && detailGeneration.current === generation)
          setSelectedProduct(refreshed);
      }
      await refreshProducts();
    } catch (cause) {
      if (activeProductId.current === productId && detailGeneration.current === generation)
        setDetailError(reviewCreated
          ? 'Đánh giá đã được gửi nhưng không thể tải lại chi tiết sản phẩm.'
          : cause instanceof Error ? cause.message : 'Không thể thêm đánh giá.');
    } finally {
      mutationInFlight.current = false;
      setIsSaving(false);
    }
  }

  return (
    <main className="storefront">
      <header className="site-header">
        <a className="brand" href="#top" aria-label="Mộc Market, trang chủ">
          <span className="brand__mark" aria-hidden="true">m.</span><span>MỘC MARKET</span>
        </a>
        <span className="site-header__note">Những món đồ nhỏ, niềm vui lớn</span>
      </header>
      <section className="intro" id="top">
        <p className="eyebrow">ĐƯỢC CHỌN LỌC BẰNG SỰ TỬ TẾ</p>
        <h1>Đồ hay cho<br /><span>những ngày thường.</span></h1>
        <p className="intro__description">Khám phá những món đồ hữu ích, được làm chỉn chu để đồng hành cùng bạn mỗi ngày.</p>
        <div className="search-box">
          <span aria-hidden="true">⌕</span>
          <label className="visually-hidden" htmlFor="product-search">Tìm kiếm sản phẩm</label>
          <input id="product-search" type="search" placeholder="Bạn đang tìm món gì?" value={search} onChange={(event) => { setIsLoading(true); setError(null); setSearch(event.target.value); }} />
          <kbd>/</kbd>
        </div>
      </section>
      <section className="catalog" aria-labelledby="catalog-title">
        <div className="section-heading">
          <div><p className="eyebrow">BỘ SƯU TẬP NHỎ</p><h2 id="catalog-title">Sản phẩm nổi bật</h2></div>
          <div className="catalog-actions"><p className="section-heading__count">{products.length} sản phẩm</p><button className="button button--solid" type="button" disabled={isSaving} onClick={() => { setSelectedProduct(null); setIsEditing(false); setIsCreating(true); setDetailError(null); }}>+ Thêm sản phẩm</button></div>
        </div>
        {isLoading ? <p className="status-message" role="status">Đang tải sản phẩm…</p> : null}
        {error ? <p className="status-message status-message--error" role="alert">{error}</p> : null}
        {!isLoading && !error ? <ProductList products={products} onViewDetail={openProduct} /> : null}
      </section>
      <footer className="site-footer">
        <span className="brand__mark" aria-hidden="true">m.</span><p>Mua ít hơn. Chọn điều tốt hơn.</p><span>© 2026 Mộc Market</span>
      </footer>
      {isCreating || isEditing ? (
        <div className="dialog-backdrop" role="presentation" onClick={() => { if (!isSaving) { setIsCreating(false); setIsEditing(false); } }}>
          <section aria-modal="true" className="detail-dialog product-editor" onClick={(event) => event.stopPropagation()} role="dialog">
            <button className="dialog-close" type="button" disabled={isSaving} onClick={() => { setIsCreating(false); setIsEditing(false); }} aria-label="Đóng biểu mẫu">×</button>
            <ProductForm key={selectedProduct?.id ?? 'new'} product={selectedProduct} saving={isSaving} error={detailError} onCancel={() => { setIsCreating(false); setIsEditing(false); }} onChange={() => setDetailError(null)} onSubmit={saveProduct} />
          </section>
        </div>
      ) : null}
      {isLoadingDetail ? <div className="dialog-backdrop" role="presentation"><p className="status-message">Đang tải chi tiết…</p></div> : null}
      {detailError && !isCreating && !isEditing && !selectedProduct ? <p className="status-message status-message--error" role="alert">{detailError}</p> : null}
      {selectedProduct && !isEditing ? (
        <div className="dialog-backdrop" role="presentation" onClick={closeProductDetail}>
          <section aria-labelledby="detail-title" aria-modal="true" className="detail-dialog" onClick={(event) => event.stopPropagation()} role="dialog">
            <button className="dialog-close" type="button" onClick={closeProductDetail} aria-label="Đóng chi tiết">×</button>
            <div className="detail-dialog__layout">
              <img className="detail-dialog__image" src={selectedProduct.imageUrl} alt={selectedProduct.name} />
              <div className="detail-dialog__content">
                <div className="detail-dialog__heading">
                  <div>
                    <p className="eyebrow">{selectedProduct.category}</p>
                    <h2 id="detail-title">{selectedProduct.name}</h2>
                  </div>
                  <div className="product-menu">
                    <button
                      aria-expanded={isProductMenuOpen}
                      aria-controls="product-menu-items"
                      aria-label="Product actions"
                      className="product-menu__trigger"
                      disabled={isSaving}
                      onClick={() => setIsProductMenuOpen((open) => !open)}
                      ref={productMenuTrigger}
                      type="button"
                    >
                      <span aria-hidden="true">⋯</span>
                    </button>
                    {isProductMenuOpen ? (
                      <div id="product-menu-items" className="product-menu__items" aria-label="Product actions" onKeyDown={(event) => {
                        if (event.key === 'Escape') {
                          setIsProductMenuOpen(false);
                          productMenuTrigger.current?.focus();
                        }
                      }} role="group">
                        <button type="button" onClick={() => { setIsProductMenuOpen(false); setDetailError(null); setIsEditing(true); }}>
                          <span aria-hidden="true">✎</span> Chỉnh sửa sản phẩm
                        </button>
                        <button className="product-menu__delete" type="button" onClick={() => { setIsProductMenuOpen(false); removeProduct(); }}>
                          <span aria-hidden="true">⌫</span> Xóa sản phẩm
                        </button>
                      </div>
                    ) : null}
                  </div>
                </div>
                <p className="detail-dialog__price">{currency.format(selectedProduct.price)}</p>
                <p className="detail-dialog__description">{selectedProduct.description}</p>
                {detailError ? <p className="status-message status-message--error" role="alert">{detailError}</p> : null}
                <div className="reviews-heading"><h3>Đánh giá</h3><span><strong>★ {selectedProduct.rating.toFixed(1)}</strong> · {selectedProduct.reviewCount} nhận xét</span></div>
                <div className="review-list">
                  {selectedProduct.reviews.map((review) => (
                    <article className="review" key={review.id}>
                      <div className="review__meta"><strong>{review.author}</strong><span aria-label={`${review.rating} trên 5 sao`}>{'★'.repeat(review.rating)}</span></div>
                      <p>{review.comment}</p>
                      <time dateTime={review.date}>{new Date(`${review.date}T00:00:00`).toLocaleDateString('vi-VN')}</time>
                    </article>
                  ))}
                  {selectedProduct.reviewCount > selectedProduct.reviews.length ? <p className="review-limit">Đang hiển thị 100 đánh giá mới nhất.</p> : null}
                </div>
                <form className="review-form" onSubmit={submitReview}>
                  <h3>Viết đánh giá</h3>
                  <label>Tên của bạn<input required maxLength={100} disabled={isSaving} value={reviewAuthor} onChange={(event) => setReviewAuthor(event.target.value)} /></label>
                  <label>Số sao<select disabled={isSaving} value={reviewRating} onChange={(event) => setReviewRating(event.target.value)}>{[5, 4, 3, 2, 1].map((rating) => <option key={rating} value={rating}>{rating} sao</option>)}</select></label>
                  <label>Nhận xét<textarea required maxLength={2000} rows={3} disabled={isSaving} value={reviewComment} onChange={(event) => setReviewComment(event.target.value)} /></label>
                  <button className="button button--solid" type="submit" disabled={isSaving}>{isSaving ? 'Đang gửi…' : 'Gửi đánh giá'}</button>
                </form>
              </div>
            </div>
          </section>
        </div>
      ) : null}
    </main>
  );
}

export default App;
