USE [uchetPR419]
GO
/****** Object:  Trigger [dbo].[tr_DateChange]    Script Date: 05.11.2022 10:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER trigger [dbo].[tr_DateChange]
on [dbo].[products_users]
after insert , Update
as
update [dbo].[products_users]
set date = getdate()
where id = (Select id from inserted)
