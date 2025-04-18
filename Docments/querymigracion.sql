
WITH Category AS (
	SELECT  COALESCE(c.CATEGID, b.CATEGID, a.CATEGID) CATEGID,
	    COALESCE(a.CATEGNAME, '') ||
	    (CASE WHEN a.CATEGNAME IS NOT NULL AND (b.CATEGNAME IS NOT NULL OR c.CATEGNAME IS NOT NULL) THEN '::' ELSE '' END) ||
	    COALESCE(b.CATEGNAME, '') ||
	    (CASE WHEN b.CATEGNAME IS NOT NULL AND c.CATEGNAME IS NOT NULL THEN '::' ELSE '' END) ||
	    COALESCE(c.CATEGNAME, '') AS CATEGNAME
	FROM CATEGORY_V1 a
	LEFT JOIN CATEGORY_V1 b ON b.PARENTID = a.CATEGID
	LEFT JOIN CATEGORY_V1 c ON c.PARENTID = b.CATEGID
	where a.PARENTID = -1
)
select t.transid,t.TRANSDATE,t.ACCOUNTID,concat('Activos::Bancos::',a.ACCOUNTNAME) as account ,t.notes,c.CATEGID,c.CATEGNAME,t.TRANSAMOUNT 
from CHECKINGACCOUNT_V1 t
	inner join Category c on c.CATEGID = t.CATEGID 
	inner join ACCOUNTLIST_V1 a on a.ACCOUNTID = t.ACCOUNTID
where t.TRANSCODE = 'Deposit'
UNION 
select t.transid,t.TRANSDATE,t.ACCOUNTID,concat('Activos::Bancos::',a.ACCOUNTNAME) ,t.notes,c.CATEGID,c.CATEGNAME,-t.TRANSAMOUNT 
from CHECKINGACCOUNT_V1 t
	inner join Category c on c.CATEGID = t.CATEGID 
	inner join ACCOUNTLIST_V1 a on a.ACCOUNTID = t.ACCOUNTID
where t.TRANSCODE = 'Withdrawal'
union
select t.transid,t.TRANSDATE,t.ACCOUNTID,concat('Activos::Bancos::',a.ACCOUNTNAME) ,t.notes,a2.ACCOUNTID,concat('Activos::Bancos::',a2.ACCOUNTNAME) as account2,-t.TRANSAMOUNT 
from CHECKINGACCOUNT_V1 t
	inner join ACCOUNTLIST_V1 a2 on a2.ACCOUNTID = t.TOACCOUNTID
	inner join ACCOUNTLIST_V1 a on a.ACCOUNTID = t.ACCOUNTID
where t.TRANSCODE = 'Transfer'




WITH Category AS (
	SELECT  COALESCE(c.CATEGID, b.CATEGID, a.CATEGID) CATEGID,
	    COALESCE(a.CATEGNAME, '') ||
	    (CASE WHEN a.CATEGNAME IS NOT NULL AND (b.CATEGNAME IS NOT NULL OR c.CATEGNAME IS NOT NULL) THEN '::' ELSE '' END) ||
	    COALESCE(b.CATEGNAME, '') ||
	    (CASE WHEN b.CATEGNAME IS NOT NULL AND c.CATEGNAME IS NOT NULL THEN '::' ELSE '' END) ||
	    COALESCE(c.CATEGNAME, '') AS CATEGNAME
	FROM CATEGORY_V1 a
	LEFT JOIN CATEGORY_V1 b ON b.PARENTID = a.CATEGID
	LEFT JOIN CATEGORY_V1 c ON c.PARENTID = b.CATEGID
	where a.PARENTID = -1
)
select t.transid,s.SPLITTRANSID ,t.TRANSDATE,t.ACCOUNTID,concat('Activos::Bancos::',a.ACCOUNTNAME) as Account ,t.notes,c.CATEGID,c.CATEGNAME,s.SPLITTRANSAMOUNT  
from CHECKINGACCOUNT_V1 t
	inner join SPLITTRANSACTIONS_V1 s on s.TRANSID = t.TRANSID 
	inner join ACCOUNTLIST_V1 a on a.ACCOUNTID = t.ACCOUNTID
	inner join Category c on c.CATEGID = s.CATEGID 
