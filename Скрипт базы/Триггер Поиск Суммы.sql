USE [uchetPR419]
GO
/****** Object:  Trigger [dbo].[tr_SumFinder]    Script Date: 05.11.2022 10:27:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER trigger [dbo].[tr_SumFinder]
on [dbo].[products_users]
after insert
as
begin
	declare @count int,
	@price int
	
	set @count = (select count from inserted)
	set @price = (select price from inserted)

	update products_users
	set sum = @count * @price
	where id = (select id from inserted)
	
end
