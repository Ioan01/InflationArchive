import { ProductModel } from "@/models/ProductModel";

export interface QueryResponseModel {
  products: ProductModel[];
  totalCount: number;
}
