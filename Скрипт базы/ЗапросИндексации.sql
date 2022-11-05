update products_users
set name_payment = 
(
	select top 100000 CONCAT(SUBSTRING(c.category_name,1,1),'-',n.number,'-',pu1.date)
	from  ((products_users as pu1 inner join products as p on pu1.product_id = p.id) inner join category as c on p.category_id = c.id)
	inner join 
	(
		select top 100000 ROW_NUMBER() over(partition by pu2.user_id order by pu2.user_id, co.category_name)  as 'number', pu2.id
		from (products_users as pu2 inner join products as pr on pu2.product_id = pr.id) inner join category as co on pr.category_id = co.id
		order by pu2.user_id, co.category_name, pu2.date
		
	) as n on n.id = pu1.id
	where products_users.id = pu1.id
	order by pu1.user_id, c.category_name, pu1.date
	
)