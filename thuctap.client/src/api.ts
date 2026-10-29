import type { Product, ProductInput, ProductSummary, Review, ReviewInput } from './types';

const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '');

async function send<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers: { Accept: 'application/json', ...(init?.body ? { 'Content-Type': 'application/json' } : {}), ...init?.headers },
  });

  if (!response.ok) {
    const body = await response.json().catch(() => null) as { title?: string; errors?: Record<string, string[]> } | null;
    const validation = body?.errors ? Object.values(body.errors).flat().join(' ') : '';
    throw new Error(validation || body?.title || `Yêu cầu thất bại (HTTP ${response.status}).`);
  }

  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export function getProducts(search: string, signal?: AbortSignal): Promise<ProductSummary[]> {
  const query = search.trim() ? `?search=${encodeURIComponent(search.trim())}` : '';
  return send<ProductSummary[]>(`/api/products${query}`, { signal });
}

export function getProduct(id: string, signal?: AbortSignal): Promise<Product> {
  return send<Product>(`/api/products/${encodeURIComponent(id)}`, { signal });
}

export function createProduct(input: ProductInput): Promise<Product> {
  return send<Product>('/api/products', { method: 'POST', body: JSON.stringify(input) });
}

export function updateProduct(id: string, input: ProductInput): Promise<Product> {
  return send<Product>(`/api/products/${encodeURIComponent(id)}`, { method: 'PUT', body: JSON.stringify(input) });
}

export function deleteProduct(id: string): Promise<void> {
  return send<void>(`/api/products/${encodeURIComponent(id)}`, { method: 'DELETE' });
}

export function addReview(id: string, input: ReviewInput): Promise<Review> {
  return send<Review>(`/api/products/${encodeURIComponent(id)}/reviews`, { method: 'POST', body: JSON.stringify(input) });
}
