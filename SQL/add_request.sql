CREATE OR REPLACE FUNCTION he.add_request(p_id_request_type character varying, p_input json, p_id_ws_client integer)
  RETURNS bigint AS
$BODY$

DECLARE
	v_id_request bigint;
BEGIN
	
	INSERT INTO he.requests(id_request_type, request_input, id_ws_client) VALUES 
		(p_id_request_type, p_input, p_id_ws_client) RETURNING id INTO v_id_request;

	INSERT INTO he.requests_queue(id, id_status, id_request_type) VALUES (v_id_request, 'WAIT', p_id_request_type);

	NOTIFY "new post arrived";

	return v_id_request;
END
$BODY$
  LANGUAGE plpgsql VOLATILE COST 100;
GRANT EXECUTE ON FUNCTION he.add_request(character varying, json, integer) TO gugleus;