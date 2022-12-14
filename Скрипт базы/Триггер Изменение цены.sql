USE [uchetPR419]
GO
/****** Object:  Trigger [dbo].[tr_PriceChange]    Script Date: 05.11.2022 10:27:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER trigger [dbo].[tr_PriceChange]
on [dbo].[products_users]
after insert , Update
as
update [dbo].[products]
set price = 
(select price  from [dbo].[products_users]
where id = (Select id from inserted)
)
where id = (Select product_id from inserted)
