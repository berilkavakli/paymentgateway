
DROP TABLE IF EXISTS public."Payment";
DROP TABLE IF EXISTS public."CardInformation";

CREATE TABLE IF NOT EXISTS public."CardInformation"
(
	"Id" SERIAL PRIMARY KEY,
    "CardNumber" text COLLATE pg_catalog."default" NOT NULL,
    "ExpiryMonth" integer NOT NULL,
    "ExpiryYear" integer NOT NULL,    
    "Cvv" integer
)

TABLESPACE pg_default;

ALTER TABLE public."CardInformation"
    OWNER to postgres;
	

CREATE TABLE IF NOT EXISTS public."Payment"
(
    "Id" SERIAL PRIMARY KEY,
    "Code" uuid,
    "Amount" double precision,
    "Currency" text COLLATE pg_catalog."default",
    "CardId" bigint,
    "Status" boolean,
    "Message" text COLLATE pg_catalog."default",
    CONSTRAINT "FK_CardId" FOREIGN KEY ("CardId")
        REFERENCES public."CardInformation" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE public."Payment"
    OWNER to postgres;


CREATE INDEX IF NOT EXISTS "fki_FK_CardId"
    ON public."Payment" USING btree
    ("CardId" ASC NULLS LAST)
    TABLESPACE pg_default;
	