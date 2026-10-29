import { useState, type FormEvent } from 'react';
import type { Product, ProductInput } from './types';

interface ProductFormProps {
  product?: Product | null;
  saving: boolean;
  error: string | null;
  onCancel: () => void;
  onChange: () => void;
  onSubmit: (input: ProductInput) => void;
}

function ProductForm({ product, saving, error, onCancel, onChange, onSubmit }: ProductFormProps) {
  const [name, setName] = useState(product?.name ?? '');
  const [category, setCategory] = useState(product?.category ?? '');
  const [price, setPrice] = useState(product?.price.toString() ?? '0');
  const [imageUrl, setImageUrl] = useState(product?.imageUrl ?? '');
  const [description, setDescription] = useState(product?.description ?? '');

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    onSubmit({ name, category, price: Number(price), imageUrl, description });
  }

  return (
    <form className="product-form" onSubmit={submit}>
      <p className="eyebrow">{product ? 'CHỈNH SỬA' : 'SẢN PHẨM MỚI'}</p>
      <h2>{product ? 'Cập nhật sản phẩm' : 'Thêm sản phẩm'}</h2>
      <label>Tên sản phẩm<input required maxLength={160} disabled={saving} value={name} onChange={(event) => { setName(event.target.value); onChange(); }} /></label>
      <label>Danh mục<input required maxLength={80} disabled={saving} value={category} onChange={(event) => { setCategory(event.target.value); onChange(); }} /></label>
      <label>Giá (VND)<input required type="number" min="0" step="0.01" max="9999999999999999" disabled={saving} value={price} onChange={(event) => { setPrice(event.target.value); onChange(); }} /></label>
      <label>Đường dẫn hình ảnh<input required type="url" maxLength={2048} disabled={saving} value={imageUrl} onChange={(event) => { setImageUrl(event.target.value); onChange(); }} /></label>
      <label>Mô tả<textarea required maxLength={4000} rows={3} disabled={saving} value={description} onChange={(event) => { setDescription(event.target.value); onChange(); }} /></label>
      {error ? <p className="status-message status-message--error" role="alert">{error}</p> : null}
      <div className="form-actions">
        <button className="button button--text" type="button" disabled={saving} onClick={onCancel}>Hủy</button>
        <button className="button button--solid" type="submit" disabled={saving}>{saving ? 'Đang lưu…' : 'Lưu sản phẩm'}</button>
      </div>
    </form>
  );
}

export default ProductForm;
