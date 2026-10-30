import { memo } from 'react';
import ProductCard from './Product';
import type { ProductSummary } from './types';

interface ProductListProps {
  products: ProductSummary[];
  onViewDetail: (productId: string) => void;
}

function ProductList({ products, onViewDetail }: ProductListProps) {
  if (products.length === 0) return <p className="empty-state">Chưa tìm thấy sản phẩm phù hợp.</p>;

  return (
    <div className="product-grid">
      {products.map((product) => (
        <ProductCard key={product.id} product={product} onViewDetail={onViewDetail} />
      ))}
    </div>
  );
}

export default memo(ProductList);
