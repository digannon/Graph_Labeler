Instructions:
Pull GraphLabeler.sln
Run Application!


Notes:
Nearly Graceful can cheat and use the extra allowed node label to create a sigma labeling.
  Best way to avoid it is to lock in node labels that create the long edge
If there is no solution, application will run forever until told to stop
There is a rare bug where sometimes the stop button doesn't go back to solve and the application must be restarted,
  not sure why



2-Regular Graph Input:
',' seperated integers, cycles sizes (spaces are ignored on all inputs)

2-Regular Node Lock:
',' seperated, CycleIndex.InnerIndex=NodeLabel
You can also node lock the same way as in general graphs



General graph input:
'|' seperates each node, each node has a ',' seperated list of adjacent nodes
List every edge exactly once, in either direction
EG
1,3|2,4|0,5|4|5|3 and 1,2,3|2,4|5|4,5|5| are the same graph

Directed graph input:
Same as general graph input except arrow goes from node slot to listed adjacent node

Node Lock:
',' seperated, NodeIndex=Label




Given an input graph, G, it searches for a Rho, Sigma, or Graceful labeling of G.
See below for an exanpation of each labling.

If G is 2-regular (the union of cycles), then simply input the list of each cycle size in line 12.
On line 15, input 0, 1, or 2, for Rho, Sigma, or Graceful labeling, respectively.

If G is not 2-regular, create an 'int[][] neighbor_lists' 
where the i'th array is the neighbor list for node i
and make 'neighbor_lists' the first input to new Graph() on line 17.

If G has n edges:
  Rho Labeling:
    Label each node with an integer in [0, 2n] such that:
      1. Each Node label is unique
      2. Each edge length is unique
         Where the length of an edge connecting node labels
         'a' and 'b' is min{ |a-b|, 2n+1-|a-b| }
    For more information on Rho Lablings:
      http://math.illinoisstate.edu/reu/Art063.RhoLabe2-Regs-AJC.pdf
  Sigma Labeling:
    Label each node with an integer in [0, 2n] such that:
      1. Each Node label is unique
      2. Each edge lable is unique and in [1, n]
         Where the lable of an edge connecting node labels
         'a' and 'b' is |a-b|
  Graceful Labeling:
    Label each with an integer in [0, n] such that:
      1. Each node label is unique
      2. Each edge lable is unique and in [1, n]
         Where the lable of an edge connecting node labels
         'a' and 'b' is |a-b|

Algorithm:
  We maintain a 'bad_node_list'. A node, N, is placed into 'bad_node_list'
  once if multiple nodes are labled as N.label and once for each non-unique
  edge adjacent to N. The goal of the algorithm is to empty the 'bad_node_list'.

  Each node is initialized to a random label.
  Update
  