using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker worker;
        Graph graph;
        bool solve_switch;
        bool two_regular_swtich;
        bool directed_switch;

        public MainWindow()
        {
            solve_switch = true;
            two_regular_swtich = true;
            directed_switch = false;
            InitializeComponent();
        }

        private void Example_Button_Action(object sender, RoutedEventArgs e)
        {
            switch (two_regular_swtich)
            {
                case true:
                    switch (LabelType_Box.SelectedIndex)
                    {
                        case 0: //Rho
                            Input_Box.Text = "3,4,5,6,7,8,9,10,11,12";
                            NodeLock_Box.Text = "0.0=0, 0.1=1, 1.0=2";
                            break;
                        case 1: //Nearly Sigma
                            Input_Box.Text = "4,8,11,3";
                            NodeLock_Box.Text = "0.0=0, 0.1=1, 1.0=2";
                            break;
                        case 2: //Sigma
                            Input_Box.Text = "4,6,8,10";
                            NodeLock_Box.Text = "0.0=0, 0.1=1, 1.0=2";
                            break;
                        case 3: //Nearly Graceful
                            Input_Box.Text = "6,5,4,3";
                            NodeLock_Box.Text = "0.0=0, 0.1=19";
                            break;
                        case 4: //Graceful
                            Input_Box.Text = "8,4,3";
                            NodeLock_Box.Text = "0.0=0, 0.1=15";
                            break;
                    }
                    break;
                case false:
                    int multiplicity = Convert.ToInt32(Multi_Block.Text);
                    if(multiplicity == 1)
                    {
                        switch (directed_switch)
                        {
                            case true:
                                Input_Box.Text = "1,3|2,4|0,5|0,4|1,5|2,3";
                                NodeLock_Box.Text = "0=0";
                                break;
                            case false:
                                switch (LabelType_Box.SelectedIndex)
                                {
                                    case 0: //Rho
                                        Input_Box.Text = "1,3|2,4|0,5|4|5|3";
                                        NodeLock_Box.Text = "0=0, 1=1";
                                        break;
                                    case 1: //Nearly Sigma
                                        Input_Box.Text = "1,2,3|2,4|5|4,5|5| ";
                                        NodeLock_Box.Text = "0=0, 1=10";
                                        break;
                                    case 2: //Sigma
                                        Input_Box.Text = "|0|0,1|0|1,3|2,3,4";
                                        NodeLock_Box.Text = "0=0, 1=1";
                                        break;
                                    case 3: //Nearly Graceful
                                        Input_Box.Text = "1,2| |1|0,4,5|1,5|2";
                                        NodeLock_Box.Text = "0=0, 1=10";
                                        break;
                                    case 4: //Graceful
                                        Input_Box.Text = " |0,2,4|0|0|3|2,3,4";
                                        NodeLock_Box.Text = "0=0, 1=9";
                                        break;
                                }
                                break;
                        }
                    }
                    else if(multiplicity == 2)
                    {
                        switch (directed_switch)
                        {
                            case true:
                                Input_Box.Text = "1,3|2,4|0,5|0,4|1,5|2,3";
                                NodeLock_Box.Text = "0=0";
                                break;
                            case false:
                                switch (LabelType_Box.SelectedIndex)
                                {
                                    case 0: //Rho
                                        Input_Box.Text = "1,3|2,4|0,5|0,4|1,5|2,3";
                                        NodeLock_Box.Text = "0=0";
                                        break;
                                    case 1: //Nearly Sigma
                                        Input_Box.Text = "1,3|2,4|0,5|0,4|1,5|2,3";
                                        NodeLock_Box.Text = "0=0, 3=7";
                                        break;
                                    case 2: //Sigma
                                        Input_Box.Text = "1,3|2,4|0,5|0,4|1,5|2,3";
                                        NodeLock_Box.Text = "0=0, 3=6";
                                        break;
                                    case 3: //Nearly Graceful
                                        Input_Box.Text = "1,3|2,4|0,5|0,4|1,5|2,3";
                                        NodeLock_Box.Text = "0=0, 3=7";
                                        break;
                                    case 4: //Graceful
                                        Input_Box.Text = "1,3|2,4|0,5|0,4|1,5|2,3";
                                        NodeLock_Box.Text = "0=0, 3=6";
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        Error_Block.Text = "No examples for this.";
                    }
                    break;
            }
        }

        private int[][] Cycles_Builder(int[] cycle_sizes)
        {
            int i, j, index, tail;
            int num_nodes = 0;
            for (i = 0; i < cycle_sizes.Length; i++)
                num_nodes += cycle_sizes[i];
            int[][] cycles = new int[num_nodes][];

            // Only left to right edges
            index = 0;
            for (i = 0; i < cycle_sizes.Length; i++)
            {
                cycles[index] = new int[2];
                index++;
                for (j = 1; j < cycle_sizes[i] - 1; j++)
                {
                    cycles[index] = new int[1];
                    index++;
                }
                cycles[index] = new int[0];
                index++;
            }

            index = 0;
            tail = -1;
            for (i = 0; i < cycle_sizes.Length; i++)
            {
                tail += cycle_sizes[i];
                cycles[index][0] = index + 1;
                cycles[index][1] = tail;
                index++;
                for (j = 1; j < cycle_sizes[i] - 1; j++)
                {
                    cycles[index][0] = index + 1;
                    index++;
                }
                index++;
            }
            return cycles;
        }

        private void Cycle_Builder_Test(int[][] node_adjacency)
        {
            int i, j;
            StringBuilder sb = new StringBuilder();

            for (i = 0; i < node_adjacency.Length; i++)
            {
                sb.Append(i + "\t");
                for (j = 0; j < node_adjacency[i].Length; j++)
                {
                    sb.Append(node_adjacency[i][j] + ", ");
                }
                sb.Append("\n");
            }
            Output_Box.Text = sb.ToString();
        }

        private void Solve_Switch()
        {
            switch (solve_switch)
            {
                case true:
                    Solve_Button.Content = "Stop!";
                    break;

                case false:
                    Solve_Button.Content = "Solve!";
                    break;
            }
            solve_switch = !solve_switch;
        }

        private void Up_Button_Action(object sender, RoutedEventArgs e)
        {
            if (!two_regular_swtich)
            {
                int multiplicity = Convert.ToInt32(Multi_Block.Text);
                multiplicity++;
                Multi_Block.Text = Convert.ToString(multiplicity);
            }
        }

        private void Down_Button_Action(object sender, RoutedEventArgs e)
        {
            if (!two_regular_swtich)
            {
                int multiplicity = Convert.ToInt32(Multi_Block.Text);
                if (multiplicity > 1)
                    multiplicity--;
                Multi_Block.Text = Convert.ToString(multiplicity);
            }
        }

        private void Directed_Button_Action(object sender, RoutedEventArgs e)
        {
            if (!two_regular_swtich)
            {
                switch (directed_switch)
                {
                    case true:
                        Directed_Button.Content = "Undirected";
                        LabelType_Box.Visibility = Visibility.Visible;
                        break;

                    case false:
                        Directed_Button.Content = "Directed";
                        LabelType_Box.Visibility = Visibility.Hidden;
                        break;
                }
                directed_switch = !directed_switch;
            }
        }

        private void General_Button_Action(object sender, RoutedEventArgs e)
        {
            switch (two_regular_swtich)
            {
                case true:
                    General_Button.Content = "General";
                    Directed_Button.Visibility = Visibility.Visible;
                    Multiplicity_Block.Visibility = Visibility.Visible;
                    Multi_Block.Visibility = Visibility.Visible;
                    Up_Button.Visibility = Visibility.Visible;
                    Down_Button.Visibility = Visibility.Visible;
                    break;

                case false:
                    General_Button.Content = "2-Regular";
                    directed_switch = false;
                    Directed_Button.Visibility = Visibility.Hidden;
                    Multiplicity_Block.Visibility = Visibility.Hidden;
                    Multi_Block.Visibility = Visibility.Hidden;
                    Up_Button.Visibility = Visibility.Hidden;
                    Down_Button.Visibility = Visibility.Hidden;
                    LabelType_Box.Visibility = Visibility.Visible;
                    break;
            }
            two_regular_swtich = !two_regular_swtich;
        }

        private void Solve_Button_Action(object sender, RoutedEventArgs e)
        {
            switch (solve_switch)
            {
                case true:
                    try
                    {
                        Solve_Button_Start();
                    }
                    catch
                    {
                        Error_Block.Text = "Something wrong with inputs!";
                    }
                    
                    break;

                case false:
                    worker.CancelAsync();
                    Error_Block.Text = "Cancelled before finished";
                    break;
            }
        }

        private void Solve_Button_Start()
        {
            switch (two_regular_swtich)
            {
                case true:
                    Prep_Two_Regular_Graph();
                    break;

                case false:
                    Prep_Grpah();
                    break;
            }
        }

        private void Prep_Grpah()
        {
            int[][] node_adjacency = Parse_Input_To_Adjacency();
            if (node_adjacency != null)
            {
                switch (directed_switch)
                {
                    case true:
                        graph = new Directed_Graph(node_adjacency, Convert.ToInt32(Multi_Block.Text));
                        break;
                    case false:
                        switch (LabelType_Box.SelectedIndex)
                        {
                            case 0: //Rho
                                graph = new Graph(node_adjacency, Convert.ToInt32(Multi_Block.Text));
                                break;
                            case 1: //Nearly Sigma
                                graph = new Nearly_Sigma_Graph(node_adjacency, Convert.ToInt32(Multi_Block.Text));
                                break;
                            case 2: //Sigma
                                graph = new Sigma_Graph(node_adjacency, Convert.ToInt32(Multi_Block.Text));
                                break;
                            case 3: //Nearly Graceful
                                graph = new Nearly_Graceful_Graph(node_adjacency, Convert.ToInt32(Multi_Block.Text));
                                break;
                            case 4: //Graceful
                                graph = new Graceful_Graph(node_adjacency, Convert.ToInt32(Multi_Block.Text));
                                break;
                        }
                        break;
                }
                Solve_Graph();
            }
            else
            {
                Error_Block.Text = "Error with Graph Input!";
            }
        }


        private void Graph_Builder_Test()
        {
            int i, j;
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < graph.node_heap.Length; i++)
            {
                sb.Append(i + ":\t");
                foreach (Edge left_edge in graph.node_heap[i].left_edges)
                    sb.Append(left_edge.left_node.index + ", ");
                sb.Append(" | ");
                foreach (Edge right_edge in graph.node_heap[i].right_edges)
                    sb.Append(right_edge.right_node.index + ", ");
                sb.Append("\n");
            }
            Output_Box.Text = sb.ToString();
        }

        private void Prep_Two_Regular_Graph()
        {
            int[] cycle_sizes = Parse_Input_To_Cycles();
            if (cycle_sizes != null)
            {
                int[][] node_adjacency = Cycles_Builder(cycle_sizes);
                switch (LabelType_Box.SelectedIndex)
                {
                    case 0: //Rho
                        graph = new Two_Regular_Graph(node_adjacency, 1, cycle_sizes);
                        break;
                    case 1: //Nearly Sigma
                        graph = new Two_Regular_Nearly_Sigma_Graph(node_adjacency, 1, cycle_sizes);
                        break;
                    case 2: //Sigma
                        graph = new Two_Regular_Sigma_Graph(node_adjacency, 1, cycle_sizes);
                        break;
                    case 3: //Nearly Graceful
                        graph = new Two_Regular_Nearly_Graceful_Graph(node_adjacency, 1, cycle_sizes);
                        break;
                    case 4: //Graceful
                        graph = new Two_Regular_Graceful_Graph(node_adjacency, 1, cycle_sizes);
                        break;
                }
                Solve_Graph();
            }
            else
            {
                Error_Block.Text = "Error with Graph Input!";
            }
        }

        private void Solve_Graph()
        {
            int[,] locked_labels = Parse_Locked_Labels();
            if (locked_labels != null)
            {
                Error_Block.Text = "";
                graph.Label_Locked_Nodes(locked_labels);
                graph.Initalize_Random_Labels();

                worker = new BackgroundWorker();
                worker.DoWork += Solve;
                worker.WorkerSupportsCancellation = true;
                worker.RunWorkerCompleted += Worker_Completed;
                worker.RunWorkerAsync();
                Solve_Switch();
            }
            else
            {
                Error_Block.Text = "Error with Node Lock!";
            }
        }

        private void Solve(object sender, DoWorkEventArgs e)
        {
            while (graph.bad_node_list.Count > 0 && !worker.CancellationPending)
            {
                graph.Update();
            }
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Output_Box.Text = graph.GetAnswer();
            Stat_Block.Text = "K" + graph.GetK() + " Attempts: " + graph.Attempts();
            Solve_Switch();
        }

        private int[] Parse_Input_To_Cycles()
        {
            try
            {
                string input = Input_Box.Text.Replace(" ", "");
                char[] delimit = { ',' };
                string[] split = input.Split(delimit);
                int[] cycle_sizes = new int[split.Length];
                for (int i = 0; i < split.Length; i++)
                {
                    cycle_sizes[i] = Convert.ToInt32(split[i]);
                    if (cycle_sizes[i] < 3)
                        return null;
                }
                return cycle_sizes;
            }
            catch
            {
                return null;
            }
        }

        private int[][] Parse_Input_To_Adjacency()
        {
            try
            {
                int i, j;
                string input = Input_Box.Text.Replace(" ", "");
                char[] delimit1 = { '|' };
                char[] delimit2 = { ',' };

                string[] split1 = input.Split(delimit1);
                string[] split2;

                int[][] node_adjacency = new int[split1.Length][];
                for (i = 0; i < split1.Length; i++)
                {
                    split2 = split1[i].Split(delimit2);
                    if (split2[0].Equals(""))
                        split2 = new string[0];
                    node_adjacency[i] = new int[split2.Length];
                    for (j = 0; j < node_adjacency[i].Length; j++)
                    {
                        node_adjacency[i][j] = Convert.ToInt32(split2[j]);
                    }
                }
                return node_adjacency;
            }
            catch
            {
                return null;
            }
        }

        private int[,] Parse_Locked_Labels()
        {
            try
            {
                string input = NodeLock_Box.Text.Replace(" ", "");
                int[,] locked_labels;
                if (input.Length == 0)
                    locked_labels = new int[0, 0];

                else
                {
                    int i, j;

                    char[] delimit1 = { ',' };
                    char[] delimit2 = { '=' };

                    string[] split1 = input.Split(delimit1);
                    string[] split2;

                    locked_labels = new int[split1.Length, 2];

                    if (two_regular_swtich && split1[0].Contains('.'))
                    {
                        char[] delimit3 = { '.' };
                        string[] split3;
                        int[] converted_to_ints = new int[2];
                        for (i = 0; i < split1.Length; i++)
                        {
                            split2 = split1[i].Split(delimit2);
                            split3 = split2[0].Split(delimit3);
                            converted_to_ints[0] = Convert.ToInt32(split3[0]);
                            converted_to_ints[1] = Convert.ToInt32(split3[1]);
                            if (converted_to_ints[0] >= graph.cycle_sizes.Length || converted_to_ints[1] >= graph.cycle_sizes[converted_to_ints[0]])
                                return null;
                            locked_labels[i, 0] = 0;
                            for (j = 0; j < converted_to_ints[0]; j++)
                                locked_labels[i, 0] += graph.cycle_sizes[j];
                            locked_labels[i, 0] += converted_to_ints[1];
                            locked_labels[i, 1] = Convert.ToInt32(split2[1]);
                            if (locked_labels[i, 1] >= graph.GetK())
                                return null;
                        }
                    }

                    else
                    {
                        for (i = 0; i < split1.Length; i++)
                        {
                            split2 = split1[i].Split(delimit2);
                            locked_labels[i, 0] = Convert.ToInt32(split2[0]);
                            locked_labels[i, 1] = Convert.ToInt32(split2[1]);
                            if (locked_labels[i, 0] >= graph.Num_Nodes() || locked_labels[i, 1] >= graph.GetK())
                                return null;
                        }
                    }
                }
                return locked_labels;
            }
            catch
            {
                return null;
            }
        }

        private class Edge
        {
            public int length;
            public Node left_node; //Left nodes are always left in nodeheap
            public Node right_node;
        }

        private class Node
        {
            public int label;
            public List<Edge> left_edges;
            public List<Edge> right_edges;
            public int index;
            public bool locked;
        }


        private class Graph
        {
            public Node[] node_heap;
            public Edge[] edge_heap;
            public List<Node> bad_node_list;
            protected Dictionary<int, List<Node>> label_to_node_list;
            protected Dictionary<int, List<Edge>> length_to_edge_list;
            protected Random randy;
            protected double p_of_accept_bad_update;
            protected double update_p_size;
            protected int K;
            protected int multiplicity;
            protected int num_edges;
            protected int attempts;
            public int[] cycle_sizes;
            public int GetK() { return K; }
            public int Attempts() { return attempts; }
            public int Num_Nodes() { return node_heap.Length; }

            //Assumes node_adjacency only shows edges from left to right
            public Graph(int[][] node_adjacency, int multiplicity, int[] cycle_sizes = null)
            {
                int i;

                // This update rule was chosen quickly from a few runtime experiements
                // It can likely be improved
                update_p_size = 1 / Math.Pow(node_adjacency.Length, 2.5);
                p_of_accept_bad_update = 0.0;

                num_edges = 0;
                for (i = 0; i < node_adjacency.Length; i++)
                    num_edges += node_adjacency[i].Length;

                this.multiplicity = multiplicity;
                this.cycle_sizes = cycle_sizes;
                Create_Graph(node_adjacency);

                K = num_edges / multiplicity * 2 + 1;
                bad_node_list = new List<Node>();
                label_to_node_list = new Dictionary<int, List<Node>>();
                for (i = 0; i < K; i++)
                    label_to_node_list.Add(i, new List<Node>());
                length_to_edge_list = new Dictionary<int, List<Edge>>();
                for (i = 0; i <= num_edges / multiplicity; i++)
                    length_to_edge_list.Add(i, new List<Edge>());
                attempts = 0;
                randy = new Random();
            }


            private void Create_Graph(int[][] node_adjacency)
            {
                int i, j;
                node_heap = new Node[node_adjacency.Length];
                for (i = 0; i < node_heap.Length; i++)
                {
                    node_heap[i] = new Node();
                    node_heap[i].index = i;
                    node_heap[i].left_edges = new List<Edge>();
                    node_heap[i].right_edges = new List<Edge>();
                }

                List<Edge> edge_list = new List<Edge>();
                Edge edge;

                //Connect Nodes and Edges
                for (i = 0; i < node_adjacency.Length; i++)
                {
                    for (j = 0; j < node_adjacency[i].Length; j++)
                    {
                        edge = new Edge();
                        node_heap[i].right_edges.Add(edge);
                        node_heap[node_adjacency[i][j]].left_edges.Add(edge);
                        edge.left_node = node_heap[i];
                        edge.right_node = node_heap[node_adjacency[i][j]];
                        edge_list.Add(edge);
                    }
                }
                edge_heap = edge_list.ToArray();
            }

            public void Label_Locked_Nodes(int[,] locked_labels)
            {
                for (int i = 0; i < locked_labels.GetLength(0); i++)
                {
                    node_heap[locked_labels[i, 0]].label = locked_labels[i, 1];
                    node_heap[locked_labels[i, 0]].locked = true;
                    Add_Node_To_List(node_heap[locked_labels[i, 0]]);
                }
            }

            protected virtual int Random_Node_Label()
            {
                return randy.Next(0, K);
            }

            public void Initalize_Random_Labels()
            {
                foreach (Node node in node_heap)
                {
                    if (!node.locked)
                    {
                        node.label = Random_Node_Label();
                        Add_Node_To_List(node);
                    }
                }

                foreach (Edge edge in edge_heap)
                {
                    edge.length = EdgeLength(edge.left_node.label, edge.right_node.label);
                    Add_Edge_To_List(edge);
                }
            }

            public void Update()
            {
                Node node;
                int old_score, old_label, new_label;
                double p = p_of_accept_bad_update;

                bool done = false;
                while (!done)
                {
                    node = bad_node_list[randy.Next(0, bad_node_list.Count)];
                    if (!node.locked)
                    {
                        old_label = node.label;
                        old_score = bad_node_list.Count;

                        new_label = Random_Node_Label();
                        Set_Node(node, new_label);

                        if (bad_node_list.Count < old_score || randy.NextDouble() < p)
                            done = true;
                        else
                        {
                            Set_Node(node, old_label);
                            p += (1 - p) * update_p_size;
                        }
                        attempts++;
                    }
                }
            }

            void Set_Node(Node node, int label)
            {
                foreach (Edge left_edge in node.left_edges)
                    Remove_Edge_From_List(left_edge);
                foreach (Edge right_edge in node.right_edges)
                    Remove_Edge_From_List(right_edge);
                Remove_Node_From_List(node);

                node.label = label;
                foreach (Edge left_edge in node.left_edges)
                    left_edge.length = EdgeLength(left_edge.left_node.label, node.label);
                foreach (Edge right_edge in node.right_edges)
                    right_edge.length = EdgeLength(node.label, right_edge.right_node.label);

                Add_Node_To_List(node);
                foreach (Edge left_edge in node.left_edges)
                    Add_Edge_To_List(left_edge);
                foreach (Edge right_edge in node.right_edges)
                    Add_Edge_To_List(right_edge);
            }

            void Add_Node_To_List(Node node)
            {
                label_to_node_list[node.label].Add(node);

                if (label_to_node_list[node.label].Count > 1)
                {
                    bad_node_list.Add(node);

                    // Add the first element as well the first time this occurs
                    if (label_to_node_list[node.label].Count == 2)
                        bad_node_list.Add(label_to_node_list[node.label][0]);
                }
            }

            void Add_Edge_To_List(Edge edge)
            {
                length_to_edge_list[edge.length].Add(edge);

                if (edge.length == 0)
                {
                    bad_node_list.Add(edge.left_node);
                    bad_node_list.Add(edge.right_node);
                }
                else if (length_to_edge_list[edge.length].Count > multiplicity)
                {
                    if (length_to_edge_list[edge.length].Count == multiplicity + 1)
                    {
                        foreach (Edge bad_edge in length_to_edge_list[edge.length])
                        {
                            bad_node_list.Add(bad_edge.left_node);
                            bad_node_list.Add(bad_edge.right_node);
                        }
                    }
                    else
                    {
                        bad_node_list.Add(edge.left_node);
                        bad_node_list.Add(edge.right_node);
                    }
                }
            }

            void Remove_Node_From_List(Node node)
            {
                label_to_node_list[node.label].Remove(node);

                if (label_to_node_list[node.label].Count > 0)
                {
                    bad_node_list.Remove(node);
                    if (label_to_node_list[node.label].Count == 1)
                        bad_node_list.Remove(label_to_node_list[node.label][0]);
                }
            }

            void Remove_Edge_From_List(Edge edge)
            {
                length_to_edge_list[edge.length].Remove(edge);

                if (edge.length == 0)
                {
                    bad_node_list.Remove(edge.left_node);
                    bad_node_list.Remove(edge.right_node);
                }
                else if (length_to_edge_list[edge.length].Count > multiplicity - 1)
                {
                    bad_node_list.Remove(edge.left_node);
                    bad_node_list.Remove(edge.right_node);

                    if (length_to_edge_list[edge.length].Count == multiplicity)
                    {
                        foreach (Edge good_edge in length_to_edge_list[edge.length])
                        {
                            bad_node_list.Remove(good_edge.left_node);
                            bad_node_list.Remove(good_edge.right_node);
                        }
                    }
                }
            }

            protected virtual int EdgeLength(int node_label_1, int node_label_2)
            {
                int label = Math.Abs(node_label_1 - node_label_2);
                return Math.Min(label, K - label);
            }

            public virtual string GetAnswer()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < node_heap.Length; i++)
                    sb.Append("Node " + i + ":\t" + node_heap[i].label + "\n");
                return sb.ToString();
            }
        }

        private class Directed_Graph : Graph
        {
            public Directed_Graph(int[][] node_adjacency, int multiplicity) : base(node_adjacency, multiplicity)
            {
                K = num_edges / multiplicity + 1;
            }

            protected override int EdgeLength(int left_node_label, int right_node_label)
            {
                int label = right_node_label - left_node_label;
                if (label < 0)
                    label += K;
                return label;
            }
        }

        private class Sigma_Graph : Graph
        {
            public Sigma_Graph(int[][] node_adjacency, int multiplicity) : base(node_adjacency, multiplicity) { }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                int label = Math.Abs(node_label_1 - node_label_2);
                if (label > edge_heap.Length/multiplicity)
                    label = 0;
                return label;
            }
        }

        private class Nearly_Sigma_Graph : Graph
        {
            public Nearly_Sigma_Graph(int[][] node_adjacency, int multiplicity) : base(node_adjacency, multiplicity) { }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                int label = Math.Abs(node_label_1 - node_label_2);
                if (label == edge_heap.Length/multiplicity + 1)
                    label--;
                if (label > edge_heap.Length/multiplicity)
                    label = 0;
                return label;
            }
        }

        private class Graceful_Graph : Graph
        {
            public Graceful_Graph(int[][] node_adjacency, int multiplicity) : base(node_adjacency, multiplicity) { }

            protected override int Random_Node_Label()
            {
                return randy.Next(0, edge_heap.Length/multiplicity + 1);
            }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                return Math.Abs(node_label_1 - node_label_2);
            }
        }

        private class Nearly_Graceful_Graph : Graph
        {
            public Nearly_Graceful_Graph(int[][] node_adjacency, int multiplicity) : base(node_adjacency, multiplicity) { }

            protected override int Random_Node_Label()
            {
                return randy.Next(0, edge_heap.Length/multiplicity + 2);
            }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                int label = Math.Abs(node_label_1 - node_label_2);
                if (label == edge_heap.Length/multiplicity + 1)
                    label--;
                return label;
            }
        }

        private class Two_Regular_Graph : Graph
        {
            public Two_Regular_Graph(int[][] node_adjacency, int multiplicity, int[] cycle_sizes) : base(node_adjacency, multiplicity, cycle_sizes) { }

            public override string GetAnswer()
            {
                StringBuilder sb = new StringBuilder();
                int cycle_index, inner_index;
                int index = 0;
                for (cycle_index = 0; cycle_index < cycle_sizes.Length; cycle_index++)
                {
                    for (inner_index = 0; inner_index < cycle_sizes[cycle_index]; inner_index++)
                    {
                        sb.Append("Node " + cycle_index + "." + inner_index + ":\t" + node_heap[index].label + "\n");
                        index++;
                    }
                    sb.Append("\n");
                }
                return sb.ToString();
            }
        }

        private class Two_Regular_Sigma_Graph : Two_Regular_Graph
        {
            public Two_Regular_Sigma_Graph(int[][] node_adjacency, int multiplicity, int[] cycle_sizes) : base(node_adjacency, multiplicity, cycle_sizes) { }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                int label = Math.Abs(node_label_1 - node_label_2);
                if (label > edge_heap.Length)
                    label = 0;
                return label;
            }
        }

        private class Two_Regular_Nearly_Sigma_Graph : Two_Regular_Sigma_Graph
        {
            public Two_Regular_Nearly_Sigma_Graph(int[][] node_adjacency, int multiplicity, int[] cycle_sizes) : base(node_adjacency, multiplicity, cycle_sizes) { }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                int label = Math.Abs(node_label_1 - node_label_2);
                if (label == edge_heap.Length + 1)
                    label--;
                if (label > edge_heap.Length)
                    label = 0;
                return label;
            }
        }

        private class Two_Regular_Graceful_Graph : Two_Regular_Graph
        {
            public Two_Regular_Graceful_Graph(int[][] node_adjacency, int multiplicity, int[] cycle_sizes) : base(node_adjacency, multiplicity, cycle_sizes) { }

            protected override int Random_Node_Label()
            {
                return randy.Next(0, edge_heap.Length + 1);
            }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                return Math.Abs(node_label_1 - node_label_2);
            }
        }

        private class Two_Regular_Nearly_Graceful_Graph : Two_Regular_Graph
        {
            public Two_Regular_Nearly_Graceful_Graph(int[][] node_adjacency, int multiplicity, int[] cycle_sizes) : base(node_adjacency, multiplicity, cycle_sizes) { }

            protected override int Random_Node_Label()
            {
                return randy.Next(0, edge_heap.Length + 2);
            }

            protected override int EdgeLength(int node_label_1, int node_label_2)
            {
                int label = Math.Abs(node_label_1 - node_label_2);
                if (label == edge_heap.Length + 1)
                    label--;
                return label;
            }
        }
    }
}
