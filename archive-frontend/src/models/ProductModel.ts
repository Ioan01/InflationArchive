import { ProductPrice } from "./ProductPrice";

export interface ProductModel {
  id: string;
  name: string;
  imageUri: string;
  pricePerUnit: number;
  unit: string;
  productPrices: ProductPrice[];
  category: string;
  manufacturer: string;
  store: string;
  isFavorite: boolean;
}
