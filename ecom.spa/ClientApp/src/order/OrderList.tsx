import { OrderDetails } from "../types/orderDetails";
import {useFetchOrderDetails} from "../hooks/orderHooks";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { OrderState } from "../types/orderState";

const OrderDetailsList = () => {
    const nav = useNavigate();
  const { data } = useFetchOrderDetails();    

    return (
        <div>
      <div className="row mb-2">
        <h5 className="themeFontColor text-center">
          Order List
        </h5>
      </div>
      <table className="table table-hover">
        <thead>
          <tr>
          <th>Order Id</th>
          <th>Customer Name</th>
          <th>Contact Number</th>
            <th>Product Name</th>
            <th>Product Price</th>
            <th>Quantity</th>
            <th>Order Status</th>            
          </tr>
        </thead>
        <tbody>
            {data &&
                data.map((o: OrderDetails) => (
                    <tr key={o.order.orderId}>
                        <td>{o.order.orderId}</td>
                        <td>{o.customerDetails.name}</td>
                        <td>{o.customerDetails.contactNumber}</td>
                        <td>{o.productDetails.name}</td>
                        <td>{o.productDetails.price}</td>
                        <td>{o.order.productCount}</td>
                        <td>{OrderState[o.order.orderState]}</td>                       

                    </tr>
                
            ))}          
        </tbody>
      </table>
    </div>
    );

};

export default OrderDetailsList