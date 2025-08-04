import { Product } from "../types/product";
import ValidationSummary from "../ValidationSummary";
import ProductForm from "./ProductForm";
import { useAddProduct } from "../hooks/ProductHooks";


const ProductAdd = () => {
    const addProductMutation = useAddProduct();
  
    const product: Product = {
        productId: "",
        name: "",
        price: 0,
        seller: "",
        availableSince: "",
        shortDescription: "",
        imageUrl: "",
        quantity: 0,
        productDescription: ""
    };

    return (
        <>
          {addProductMutation.isError && (
            <ValidationSummary error={addProductMutation.error} />
          )}
          <ProductForm
            product={product}
            submitted={(product) => addProductMutation.mutate(product)}
          />
        </>
      );
};

export default ProductAdd;