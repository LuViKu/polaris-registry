-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";
CREATE EXTENSION IF NOT EXISTS "dblink";

-- Create Hangfire database (separate from registry DB)
DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'hangfire') THEN
    PERFORM dblink_exec('dbname=postgres', 'CREATE DATABASE hangfire');
  END IF;
END
$$;
