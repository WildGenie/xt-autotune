create table genre (
  id integer primary key,
  name text not null unique
);

create index i_genre_name
on genre (name);

create table artist (
  id integer primary key,
  name text not null unique
);

create index i_artist_name
on artist (name);

create table track (
  id integer primary key,
  title text,
  genre_id integer,
  artist_id integer,
  path text not null unique,
  favourite integer not null,
  foreign key (artist_id) references artist (id)
);

create index i_track_title
on track (title);

create index i_track_path
on track (path);
