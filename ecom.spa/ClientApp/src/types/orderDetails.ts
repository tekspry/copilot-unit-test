import { Customer } from "./customer";
import { Order } from "./order"; 
import { Product } from "./product";

export type OrderDetails ={
    order: Order;
    customerDetails: Customer;
    productDetails: Product;
}