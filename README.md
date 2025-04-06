# Movie API/Database Sample

This is a sample implementation of a .NET service which wraps a MySQL database
that facilitates access to records containing details of movies.

The csv data source has been pulled from https://www.kaggle.com/datasets/disham993/9000-movies-dataset

The API exposes a search function to query movies by a set of parameters,
returning them via pagination. For simplicity, offset-based pagination has
been used. In production, depending on the use case and dataset size,
keyset pagination would be a better option.

## Build steps

* compose-db.yaml should be run first, this will build and deploy the MySql database.
* The seeder project can be run next, this will inspect movies.csv, transforming and committing it to the MySql database.
* compose-api.yaml is used to build and deploy the api. 
  * By default, the api is configured to run on port 5000 (via docker compose)
  * You can visit http://localhost:5000/api/info/index.html to get the OpenApi docs

## Endpoints

### api/movies
* Exposes search functionality to query the database based upon given query parameters.

### api/movies/{id}
* Exposes direct lookup of movies by id. A movie gets assigned a unique integer-based id when it is seeded in the database.
  * The unique id is not based upon the csv data, it is only meant for internal usage.

### api/movies/genres
* Exposes a list of possible genres. This allows a frontend UI to specify a full list of known genres.
* These values have been 'manually defined' based upon the data. This seemed acceptable to do because genres are quite a well-known and unchanging concept.
    