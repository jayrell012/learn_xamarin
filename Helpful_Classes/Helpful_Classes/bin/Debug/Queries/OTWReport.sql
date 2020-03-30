SELECT  
otw.Ref_no,  
otw.Store_Name, 
otw.Store_Id, 
otw.Supplier_Name, 
otw.Supplier_ID, 
otw.Item as Item_desc, 
otw.Item_Code, 
otw.Barcode, 
otw.Item_per_pack, 
otw.My_orders, 
otw.Date_expected, 
otw.user_ord, 
otw.Released_quantity, 
otw.encoded_quantity, 
otw.category, 
oth.Status, 
oth.Outbound_ref_no  
FROM ORDER_TO_WAREHOUSE otw  
LEFT JOIN ORDER_TW_HEADER oth ON (otw.Ref_no = oth.Ref_no) 
WHERE 
otw.DATE_EXPECTED BETWEEN '2019-08-21' AND '2019-08-27' 
AND oth.STATUS <> 'CANCELLED' 