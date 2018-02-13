\ Model Mean Angle Assignment Problem
\ LP format - for model browsing. Use MPS format to capture full model detail.
Minimize
  1.570796326794897 b0.2 + 1.570796326794897 b1.2 + 1.570796326794897 b2.0
   + 1.570796326794897 b2.1 + 1.570796326794897 b3.0
   + 1.570796326794897 b3.1
Subject To
 c0: b0.0 + b0.1 + b0.2 >= 1
 c1: b1.0 + b1.1 + b1.2 >= 1
 c2: b2.0 + b2.1 + b2.2 >= 1
 c3: b3.0 + b3.1 + b3.2 >= 1
 c4: b0.0 + b1.0 + b2.0 + b3.0 >= 1
 c5: b0.1 + b1.1 + b2.1 + b3.1 >= 1
 c6: b0.2 + b1.2 + b2.2 + b3.2 >= 1
Bounds
Binaries
 b0.0 b0.1 b0.2 b1.0 b1.1 b1.2 b2.0 b2.1 b2.2 b3.0 b3.1 b3.2
End
