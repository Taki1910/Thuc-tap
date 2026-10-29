export interface ProductSummary {
  id: string;
  name: string;
  category: string;
  price: number;
  imageUrl: string;
  rating: number;
  reviewCount: number;
}

export interface Review {
  id: string;
  author: string;
  rating: number;
  date: string;
  comment: string;
}

export interface Product extends ProductSummary {
  description: string;
  reviews: Review[];
}

export interface ProductInput {
  name: string;
  category: string;
  price: number;
  imageUrl: string;
  description: string;
}

export interface ReviewInput {
  author: string;
  rating: number;
  comment: string;
}
