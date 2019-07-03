*** License ***
Dataset from http://fenix.univ.rzeszow.pl/mkepski/ds/uf.html (licensed with Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License and intended for non-commercial academic use)

*** Data ***
adl-12-cam0-d   <<< Depth data, stored as png image sequence (250 pngs)
adl-12-cam0-rgb <<< RGB data, stored as png image sequence (250 pngs)
adl-12-acc.csv  <<< Acceleration data, may not be relevant for us
adl-12-data.csv <<< Synchronization data, may not be relevant for us

*** Remarks ***
Depth data in adl-12-cam0-d is stored in PNG16-format, we need to rescale this data as follows:
$$  d = C*P(x,y)/65535  $$
->>  d is the depth in millimeters (mm)
->>  C is the camera's scale ratio (here: C = 7000)
->>  P(x,y) is the pixel value at position (x,y) of the PNG16 image
This results in the formula
$$  7000*P(x,y)/65535 $$
to get the depth of pixel (x,y) in mm.