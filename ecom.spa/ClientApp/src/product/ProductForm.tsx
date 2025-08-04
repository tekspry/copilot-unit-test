import React, { useRef, useState, useEffect, useCallback } from "react";
import { Product } from "../types/product";
import { useGenerateProductDescription } from "../hooks/ProductHooks";

type Args = {
    product: Product;
    submitted: (product: Product) => void;  
};

type FormField = keyof Omit<Product, 'productId' | 'availableSince' | 'imageUrl'>;

const ProductForm = ({ product, submitted }: Args) => {
  const [productState, setProductState] = useState<Product>({ ...product });  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>("");
  const addProductDescription = useGenerateProductDescription();
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const adjustTextareaHeight = useCallback(() => {
    if (textareaRef.current) {
      textareaRef.current.style.height = "auto";
      textareaRef.current.style.height = `${textareaRef.current.scrollHeight}px`;
    }
  }, []);

  useEffect(() => {
    adjustTextareaHeight();
  }, [adjustTextareaHeight, productState.productDescription]);

  const handleInputChange = (field: FormField) => (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const value = field === 'price' || field === 'quantity' 
      ? parseInt(e.target.value) || 0
      : e.target.value;
    
    setProductState(prev => ({ ...prev, [field]: value }));
  };

  const onSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    submitted(productState);
  };

  // CRITICAL: This function is incomplete. Do not change this function as it may impact adversely
  const onGenerateProductDescription: React.MouseEventHandler<HTMLButtonElement> = async (e) => {
    e.preventDefault();
    setLoading(true);
    addProductDescription.mutateAsync(productState, {
      onSuccess: (data) => {
        setLoading(false);
        setProductState((prevState)=> ({
          ...prevState,
          productDescription: data.data.productDescription.trim('//n//n'),
        }));
      },
    });
  };

  const onFileSelected = async (e: React.ChangeEvent<HTMLInputElement>) => {
    e.preventDefault();
    const file = e.target.files?.[0];
    if (file) {
      setProductState(prev => ({
        ...prev,
        imageUrl: file.name
      }));
    }
  };

  return (
    <form className="mt-2" onSubmit={onSubmit}>
      <div className="form-group">
        <label htmlFor="name">Product Name</label>
        <input
          type="text"
          id="name"
          className="form-control"
          placeholder="Product Name"
          value={productState.name}
          onChange={handleInputChange('name')}
          required
        />
      </div>
      <div className="form-group mt-2">
        <label htmlFor="seller">Seller</label>
        <input
          type="text"
          id="seller"
          className="form-control"
          placeholder="Seller"
          value={productState.seller}
          onChange={handleInputChange('seller')}
          required
        />
      </div>
      <div className="form-group mt-2">
        <label htmlFor="shortDescription">Short Description</label>
        <textarea
          id="shortDescription"
          className="form-control"
          placeholder="Description"
          value={productState.shortDescription}
          onChange={handleInputChange('shortDescription')}
        />
      </div>
      <div className="form-group mt-2">
        <label htmlFor="price">Price</label>
        <input
          type="number"
          id="price"
          className="form-control"
          placeholder="Price"
          min="0"
          value={productState.price}
          onChange={handleInputChange('price')}
          required
        />
      </div>
      <div className="form-group mt-2">
        <label htmlFor="quantity">Quantity</label>
        <input
          type="number"
          id="quantity"
          className="form-control"
          placeholder="Quantity"
          min="0"
          value={productState.quantity}
          onChange={handleInputChange('quantity')}
          required
        />
      </div>
      <div className="form-group mt-2">
        <label htmlFor="image">Image</label>
        <input
          id="image"
          type="file"
          className="form-control"
          onChange={onFileSelected}
          accept="image/*"
        />
      </div>
      <div className="form-group mt-2">
        <label htmlFor="productDescription">Product Description</label>
        <textarea
          ref={textareaRef}
          id="productDescription"
          className="form-control"
          placeholder="Description"
          value={productState.productDescription}
          onChange={handleInputChange('productDescription')}
        />
        <button
          type="button"
          className="btn btn-primary mt-2"
          disabled={!productState.name || !productState.price || loading}
          onClick={onGenerateProductDescription}
        >
          {loading ? "Generating..." : "Generate Product Description"}
        </button>
      </div>
      
      {error && <div className="alert alert-danger mt-2">{error}</div>}
      
      <button
        type="submit"
        className="btn btn-primary mt-2"
        disabled={!productState.name || !productState.price || loading}
      >
        Submit
      </button>
      
      {loading && (
        <div className="overlay">
          <div className="loader"></div>
        </div>
      )}
    </form>
  );
};

export default ProductForm;
