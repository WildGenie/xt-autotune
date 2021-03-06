﻿create table genre (
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

create table album (
  id integer primary key,
  name text not null unique
);

create index i_album_name
on album (name);

create table favourite (
  id integer primary key,
  type_id text not null,
  video_id text not null
);

create unique index i_favourite_type_video
on favourite(type_id, video_id);

create table track (
  id integer primary key,
  title text,
  album_id integer,
  genre_id integer,
  artist_id integer,
  image_base64 text,
  path text not null unique,
  foreign key (artist_id) references artist (id)
);

create index i_track_title
on track (title);

create index i_track_path
on track (path);

create table suggestion (
  id integer primary key,
  title text,
  comment text,
  image_base64 text,
  type_id text not null,
  video_id text not null,
  accepted integer not null,
  declined integer not null,
  artist_id integer not null,
  foreign key (artist_id) references artist (id)
);

create unique index i_suggestion_artist_type_video
on suggestion(artist_id, type_id, video_id);
