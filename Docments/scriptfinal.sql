ATTACH DATABASE 'G:\Mi unidad\GARCA\Data\GARCA.mmb' AS otra_db;


	insert into accounts_account(id,name,parent_id)
	select categid,categname,parentid
	from CATEGORY_V1

	UPDATE accounts_account
		SET id=235
		WHERE id=1740991671610518;
	UPDATE accounts_account
		SET id=236
		WHERE id=1740991686569134;
	UPDATE accounts_account
		SET id=237,parent_id=236
		WHERE id=1740991695475131;
	UPDATE accounts_account
		SET id=238
		WHERE id=1740991705803913;
	UPDATE accounts_account
		SET id=239
		WHERE id=1740991719032044;
	UPDATE accounts_account
		SET id=240
		WHERE id=1740991731209747;
	UPDATE accounts_account
		SET id=241
		WHERE id=1740991742840362;
	UPDATE accounts_account
		SET id=242
		WHERE id=1740991757420852;
	UPDATE accounts_account
		SET id=243
		WHERE id=1740991774581036;
	UPDATE accounts_account
		SET id=244
		WHERE id=1741129751182884;

	INSERT INTO accounts_account (id,name)
	VALUES (245,'Activos');
	INSERT INTO accounts_account (id,name)
	VALUES (246,'Bancos');
	
	insert into accounts_account(id,name,parent_id)
	select accountid+246,accountname,246
	from ACCOUNTLIST_V1
	
	
	UPDATE accounts_account
	SET id=466
	WHERE id=1740990870079430;
	UPDATE accounts_account
		SET id=467
		WHERE id=1740991342044088;
	UPDATE accounts_account
		SET id=468
		WHERE id=1741040041424498;
	UPDATE accounts_account
		SET id=469
		WHERE id=1741040304911608;
	UPDATE accounts_account
		SET id=470
		WHERE id=1741040329663571;
	UPDATE accounts_account
		SET id=471
		WHERE id=1742486926515265;
	UPDATE accounts_account
		SET id=472
		WHERE id=1742487020089268;

    update accounts_account 
    set parent_id =  null
    where parent_id = -1

    insert into entries_entry(id,date,description)
    select transid,TRANSDATE,notes
    from CHECKINGACCOUNT_V1


    insert into transactions_transaction(entry_id,account_id,debit, credit)
    select t.transid,t.ACCOUNTID+246,t.TRANSAMOUNT,0
    from CHECKINGACCOUNT_V1 t
    where t.TRANSCODE = 'Deposit'
    UNION 
    select t.transid,t.ACCOUNTID+246,0,t.TRANSAMOUNT
    from CHECKINGACCOUNT_V1 t
    where t.TRANSCODE = 'Withdrawal'
    union
    select t.transid,t.ACCOUNTID+246,0,t.TRANSAMOUNT 
    from CHECKINGACCOUNT_V1 t
    where t.TRANSCODE = 'Transfer'


    insert into transactions_transaction(entry_id,account_id,debit, credit)
    select t.transid,t.CATEGID,0,t.TRANSAMOUNT 
    from CHECKINGACCOUNT_V1 t
    where t.TRANSCODE = 'Deposit'
    UNION 
    select t.transid,t.CATEGID,t.TRANSAMOUNT,0
    from CHECKINGACCOUNT_V1 t
    where t.TRANSCODE = 'Withdrawal'
    union
    select t.transid,t.TOACCOUNTID+246,t.TRANSAMOUNT,0 
    from CHECKINGACCOUNT_V1 t
    where t.TRANSCODE = 'Transfer'


    insert into transactions_transaction(entry_id,account_id,debit, credit)
    select t.transid,t.ACCOUNTID+246,sum(s.SPLITTRANSAMOUNT),0
    from CHECKINGACCOUNT_V1 t
        inner join SPLITTRANSACTIONS_V1 s on s.TRANSID = t.TRANSID
    group by t.transid,t.ACCOUNTID+246


    insert into transactions_transaction(entry_id,account_id,debit, credit)
    select t.transid,s.CATEGID,0,s.SPLITTRANSAMOUNT 
    from CHECKINGACCOUNT_V1 t
        inner join SPLITTRANSACTIONS_V1 s on s.TRANSID = t.TRANSID 


    UPDATE entries_entry
    SET description = c.payeename || ' ' || b.notes
    FROM CHECKINGACCOUNT_V1 b
    INNER JOIN PAyee_v1 c ON c.payeeid = b.payeeid
    WHERE entries_entry.id = b.transid;

    UPDATE entries_entry
    SET date = SUBSTR(date, 1, 10);

    

DETACH DATABASE otra_db;