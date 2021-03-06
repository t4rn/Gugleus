﻿CREATE ROLE gugleus_dev_user LOGIN
  ENCRYPTED PASSWORD 'md58bcdd12b689dadcbae49a1f7a17a5a89'
  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION;

-- drop schema he cascade;
create schema he;
GRANT ALL ON SCHEMA he TO gugleus_dev_user;
--********************************************************************************
CREATE TABLE he.ws_clients
(
  id serial primary key,
  client_name character varying(32) unique,
  hash character varying(32),
  ghost boolean NOT NULL DEFAULT false,
  add_date timestamp without time zone DEFAULT now()
)WITH (OIDS=FALSE);
GRANT SELECT ON TABLE he.ws_clients TO gugleus_dev_user;
INSERT INTO he.ws_clients (client_name, hash) VALUES ('TEST', 'abc');
--********************************************************************************
CREATE TABLE he.dic_request_status
(
  code character varying(4) primary key,
  description character varying(32),
  ghost boolean DEFAULT false,
  add_date timestamp without time zone DEFAULT now()
) WITH (OIDS=FALSE);
INSERT INTO he.dic_request_status (code, description) VALUES 
('WAIT', 'Waiting'),('PROC', 'Processing'),('DONE', 'Done'),('ERR', 'Error occured');
GRANT SELECT ON TABLE he.dic_request_status TO gugleus_dev_user;
--********************************************************************************
CREATE TABLE he.dic_request_type
(
  code character varying(8) primary key,
  description character varying(32),
  ghost boolean DEFAULT false,
  add_date timestamp without time zone DEFAULT now()
) WITH (OIDS=FALSE);
INSERT INTO he.dic_request_type (code, description) VALUES 
('ADDPOST', 'Add post'),('GETINFO', 'Get post info');
GRANT SELECT ON TABLE he.dic_request_type TO gugleus_dev_user;
--********************************************************************************
CREATE TABLE he.requests
(
  id bigserial PRIMARY KEY,
  id_ws_client integer references he.ws_clients,
  id_request_type character varying(8) references he.dic_request_type NOT NULL,
  request_input json,
  request_output json,
  add_date timestamp without time zone DEFAULT now(),
  output_date timestamp without time zone
) WITH (OIDS=FALSE);
GRANT ALL ON TABLE he.requests TO gugleus_dev_user;
GRANT ALL ON TABLE he.requests_id_seq TO gugleus_dev_user;
--********************************************************************************
CREATE TABLE he.requests_queue
(
  id bigint primary key references he.requests,
  id_status character varying(4) references he.dic_request_status NOT NULL,
  id_request_type character varying(8) references he.dic_request_type NOT NULL,
  add_date timestamp without time zone DEFAULT now(),
  process_start_date timestamp without time zone,
  process_end_date timestamp without time zone,
  error_msg text
)WITH (OIDS=FALSE);
GRANT ALL ON TABLE he.requests_queue TO gugleus_dev_user;