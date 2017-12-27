CREATE OR REPLACE FUNCTION he.add_request(p_id_request_type character varying, p_input json)
  RETURNS bigint AS
$BODY$

DECLARE
	v_id_request bigint;
BEGIN
	
	INSERT INTO he.requests(id_request_type, request_input) VALUES (p_id_request_type, p_input) RETURNING id INTO v_id_request;

	INSERT INTO he.requests_queue(id, id_status) VALUES (v_id_request, 'WAIT');

	NOTIFY "new post arrived";

	return v_id_request;
END
$BODY$
  LANGUAGE plpgsql VOLATILE COST 100;
GRANT EXECUTE ON FUNCTION he.add_request(character varying, json) TO gugleus;