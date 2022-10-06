update transactions 
set
	orden = date_part('year',date)*100000000000+
	date_part('month',date)*1000000000+date_part('day',date)*10000000+id*10+
	case when amount <0 then 1 else 0 end;