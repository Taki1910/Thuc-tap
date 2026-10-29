
import type { ProductSummary } from './types';

const currency = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 });

interface ProductCardProps {
  product: ProductSummary;
  onViewDetail: (productId: string) => void;
}

function ProductCard({ product, onViewDetail }: ProductCardProps) {
  return (
    <article className="product-card">
      <button className="product-card__image-button" type="button" onClick={() => onViewDetail(product.id)} aria-label={`Xem chi tiết ${product.name}`}>
        <img className="product-card__image" src={product.imageUrl} alt={product.name} loading="lazy" />
        <span className="product-card__category">{product.category}</span>
      </button>
      <div className="product-card__content">
        <h3>{product.name}</h3>
        <div className="product-card__footer">
          <p className="product-card__price">{currency.format(product.price)}</p>
          <p className="product-card__rating" aria-label={`${product.rating} trên 5 sao, ${product.reviewCount} đánh giá`}>
            <span aria-hidden="true">★</span> {product.rating.toFixed(1)} <span className="product-card__review-count">({product.reviewCount})</span>
          </p>
        </div>
        <button className="button button--text" type="button" onClick={() => onViewDetail(product.id)}>Xem chi tiết <span aria-hidden="true">↗</span></button>
      </div>
    </article>
  );
}

export default ProductCard;