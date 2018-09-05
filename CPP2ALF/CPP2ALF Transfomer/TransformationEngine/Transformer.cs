using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CPP2ALF_Transfomer.TransformationEngine
{
    class Transformer
    {
        //attributes for saving cpp alf and srcml code
        private string srcMLCodeCpp;
        private string srcMLCodeH;
        private string cppCode;
        private string alfCode;
        public customDictionary TypeRules = new customDictionary();

        public string SrcMLCodeCpp { get => srcMLCodeCpp; set => srcMLCodeCpp = value; }
        public string SrcMLCodeH { get => srcMLCodeH; set => srcMLCodeH = value; }
        public string CppCode { get => cppCode; set => cppCode = value; }
        public string AlfCode { get => alfCode; set => alfCode = value; }

        public System.Windows.Forms.RichTextBox rtb = null;

        private XmlReader cppReader;
        private XmlReader hReader;

        private string classAttributes = "";
        private string ClassName = "";
        private string superClass = "";
        private string IncludesAndTopComments = "";
        private string classBody = "";
        private string constructorInit = "";
        private Dictionary<string, string[]> ListOfFunctionsWithVisibility;
        private List<string> classAttributesList;
        private bool Custom = false;
        public string Transform(bool Custom = false)
        {
            this.Custom = Custom;
            string Result = "";
            //rtb.Text = "";
            classAttributes = "";
            ClassName = "";
            superClass = "";
            IncludesAndTopComments = "";
            classBody = "";
            constructorInit = "";
            ListOfFunctionsWithVisibility = new Dictionary<string, string[]>();
            classAttributesList = new List<string>();
            cppReader = XmlReader.Create(new StringReader(srcMLCodeCpp));
            if (Custom)
            {
                RulesForCustomCode(cppReader);
                Result += IncludesAndTopComments + "\n";
                Result += classBody;
            }
            else
            {

                if (SrcMLCodeH.Length > 0)
                {
                    hReader = XmlReader.Create(new StringReader(srcMLCodeH));

                    //using a namespace //
                    IncludesAndTopComments += "namespace DefaultPkg;\n\n";
                }
                if (srcMLCodeH.Trim().Length > 0)
                    HeaderFileReaderAndWrtingIncldesAndSettingSomeEssentialVariables(hReader);
                //else
                //  DepthOfBlocks++; //depth of classes and functions 


                RulesForCppFile(cppReader);

                string virtualFunctions = "\n";

                foreach (var vf in ListOfFunctionsWithVisibility.Where(x => x.Value[1] == "True"))
                    virtualFunctions += vf.Value[0] + " abstract " + vf.Key + "();\n";

                string AbstractClassKeyword = virtualFunctions.Trim().Length > 1 ? "abstract " : "";

                Result += IncludesAndTopComments + Environment.NewLine;
                if (ClassName == "")
                    Result += "activity main()" + Environment.NewLine;

                else
                    Result += AbstractClassKeyword + "class " + ClassName + superClass + Environment.NewLine;


                Result += "{\n" + classAttributes + virtualFunctions + classBody + "\n}";


            }
            return Result;
        }

        #region rules_writer_alf

        private int DepthOfBlocks = 0;
        private string access_modifier = "";
        private bool inClassMember = false;
        private void HeaderFileReaderAndWrtingIncldesAndSettingSomeEssentialVariables(XmlReader xReader)
        {
            while (xReader.Read())
            {
                if (xReader.NodeType == XmlNodeType.Element)
                {
                    //cpp includes////////////////////
                    //////////////////////////////////
                    if (xReader.Name == "cpp:include")
                    {
                        ReadIncludesAndWriteEquivalnetALF(xReader);
                    }
                    // Declaration Statements Reading
                    /////////////////////////////////////////////////////
                    if (xReader.Name == "decl_stmt")
                    {
                        DepthOfBlocks++;
                        classAttributes += DeclarationStatementReaderForCPP(xReader.ReadSubtree());
                        DepthOfBlocks--;
                    }

                    //Function Declaration Reading
                    /////////////////////////////////////////////////////////////
                    if (xReader.Name == "function_decl")
                    {
                        bool isVirtual = false;
                        xReader.Read();
                        if (xReader.Name == "specifier")
                        {
                            xReader.Read();
                            if (xReader.Value == "virtual")
                            {
                                isVirtual = true;
                            }
                        }

                        xReader.ReadToFollowing("name");
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        string functionName = xReader.Value;
                        if (!ListOfFunctionsWithVisibility.ContainsKey(functionName)) // added check for exception handle
                            ListOfFunctionsWithVisibility.Add(functionName, new string[] { access_modifier, isVirtual.ToString(), "function" });
                    }
                    // Desctructor Declaration Reading. Needed if there is a virtual destructor.
                    if (xReader.Name == "Destructor")
                    {
                        bool isVirtual = false;
                        xReader.Read();
                        if (xReader.Name == "specifier")
                        {
                            xReader.Read();
                            if (xReader.Value == "virtual")
                            {
                                isVirtual = true;
                            }
                        }
                        // At this point, we dont need any further information. We already know destructor Name with an empty body //
                        //ListOfFunctionsWithVisibility.Add(ClassName, new string[] { access_modifier, isVirtual.ToString(), "Destructor" });
                        //xReader.ReadToFollowing("name");
                        //xReader.ReadToFollowing("name");
                        //xReader.Read();
                        //string destructorName = xReader.Value;
                        //ListOfFunctionsWithVisibility.Add(functionName, new string[] { access_modifier, isVirtual.ToString() });
                    }
                    //class declaration Reading ///
                    ////////////////////////
                    if (xReader.Name == "class")
                    {
                        inClassMember = true;
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        ClassName = xReader.Value;
                        xReader.ReadToNextSibling("super");
                        xReader.Read();
                        if (xReader.NodeType == XmlNodeType.Whitespace)
                            xReader.Read();
                        if (xReader.Name == "super")
                        {
                            xReader.ReadToFollowing("name");
                            xReader.Read();
                            superClass = " specializes " + xReader.Value;
                        }
                    }

                    if (xReader.Name == "private")
                    {
                        access_modifier = "private ";
                    }
                    if (xReader.Name == "public")
                    {
                        access_modifier = "public ";
                    }
                    if (xReader.Name == "protected")
                    {
                        access_modifier = "protected ";
                    }

                }
            }
        }

        private bool IsHeaderCompleted = false;
        private bool inConstructor = false;
        private void RulesForCppFile(XmlReader xReader)
        {
            while (xReader.Read())
            {

                if (xReader.NodeType == XmlNodeType.Element) //We need Elements at the moment.
                {
                    //comment//////////////////////////
                    //////////////////////////////////
                    if (xReader.Name == "comment")
                    {
                        CommentReader(xReader);
                    }
                    //The Includes in CPP File Reading////////////////////
                    //////////////////////////////////
                    if (xReader.Name == "cpp:include")
                    {
                        ReadIncludesAndWriteEquivalnetALF(xReader);
                    }
                    ////Function Reading////////////////////////
                    /////////////////////////////////////
                    if (xReader.Name == "function")
                    {
                        IsHeaderCompleted = true;
                        if (hReader == null) //this will work for activity main
                        {
                            xReader.ReadToFollowing("name"); //got type
                            xReader.ReadToFollowing("name"); //got name
                            xReader.Read(); //got value of name
                            if (xReader.Value == "main")
                            {
                                xReader.ReadToFollowing("block"); //parameters of main skipped

                                continue;
                            }
                        }
                        xReader.ReadToFollowing("name"); //return type of function
                        xReader.Read(); //got value
                        string returnType = xReader.Value; // getting value
                        xReader.ReadToFollowing("operator");
                        xReader.ReadToFollowing("name"); //name of function
                        xReader.Read(); // Name of function
                        string nameOfFunction = xReader.Value; // getting value

                        xReader.ReadToFollowing("parameter_list");
                        string parametersText = "()";
                        if (xReader.NodeType == XmlNodeType.Element)
                        {
                            XmlReader parameterListInXML = xReader.ReadSubtree();
                            parameterListInXML.Read();
                            parametersText = $"({ParameterRules(parameterListInXML)})";
                        }


                        //appending function declaration with parametrs and return type in class body.

                        if (ListOfFunctionsWithVisibility.ContainsKey(nameOfFunction))
                        {
                            classBody += ListOfFunctionsWithVisibility[nameOfFunction][0] + nameOfFunction + parametersText + ((returnType == "void") ? "" : " : " + TypeRules.Get(returnType).alfName);
                            ListOfFunctionsWithVisibility[nameOfFunction][1] = false.ToString();
                        }
                    }
                    ////expression statement reading ////////////////////////
                    /////////////////////////////////////
                    if (xReader.Name == "expr_stmt")
                    {
                        classBody += ExpressionWriter(xReader.ReadSubtree());
                    }
                    // if and else statement reader
                    //////////////////////////////////////////////////////////
                    if (xReader.Name == "if")
                    {
                        xReader.Read();
                        xReader.Read();
                        var ifReader = xReader.ReadSubtree();
                        string conditionalExpression = conditionalExpressionReader(ifReader);
                        classBody += $"if({conditionalExpression})";
                    }
                    if (xReader.Name == "else")
                    {
                        classBody += "else";
                    }
                    // return statement reading
                    //////////////////////////////////////////
                    if (xReader.Name == "return")
                    {
                        XmlReader subDoc = xReader.ReadSubtree();
                        string text = ExpressionWriter(subDoc);
                        classBody += text;
                    }
                    // function constructor reading
                    ////////////////////////////////////////
                    if (xReader.Name == "constructor")
                    {
                        IsHeaderCompleted = true;

                        xReader.ReadToFollowing("operator"); // function name appears after operator
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        string constructorName = xReader.Value; // getting value

                        xReader.ReadToFollowing("parameter_list");
                        XmlReader subTree = xReader.ReadSubtree();
                        subTree.Read();
                        //xReader.ReadToNextSibling("parameter_list");
                        string parameters = $"({ParameterRules(subTree)})";
                        //Writng constructor in ALF Syntax $ALF Rules For Constructor$
                        classBody += "\n@Create\npublic " + constructorName + parameters;
                        inConstructor = true;
                    }
                    //// Construction Initializers 
                    /////////////////////////////
                    if (xReader.Name == "member_init_list")
                    {
                        XmlReader members_initsSubTree = xReader.ReadSubtree();
                        xReader.Read();
                        ReadConstructorInitializers(members_initsSubTree);
                    }
                    //// Destructor ////
                    if (xReader.Name == "destructor")
                    {
                        IsHeaderCompleted = true;

                        xReader.ReadToFollowing("operator"); // function name appears after operator
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        string destructorName = xReader.Value; // getting value

                        classBody += "\n@Destroy\npublic " + destructorName + "()";
                    }
                    /// The Block Reader
                    /// //////////////////
                    if (xReader.Name == "block")
                    {
                        var block = xReader.ReadSubtree();
                        block.Read();
                        classBody += "{\n";
                        if (inConstructor) //writing constructor
                        {
                            classBody += constructorInit + "\n";
                            inConstructor = false;
                        }
                        DepthOfBlocks++;
                        RulesForCppFile(block);
                        DepthOfBlocks--;
                        classBody += "\n}";
                        if (DepthOfBlocks == 0)
                            inClassMember = false;
                    }
                    // Declaration Statement ////
                    //////////////////////////
                    if (xReader.Name == "decl_stmt")
                    {
                        XmlReader declTree = xReader.ReadSubtree();
                        classBody += DeclarationStatementReaderForCPP(declTree);
                    }
                    // Enum Handling //
                    ///////////////////////////
                    if (xReader.Name == "enum")
                    {
                        //enum TBD ///
                        XmlReader enumTree = xReader.ReadSubtree();
                        while (enumTree.Read()) ;
                    }

                    // Initialization things
                    ///////////////////////////////////
                    if (xReader.Name == "init")
                    {
                        XmlReader initPath = xReader.ReadSubtree();
                        string result = ExpressionWriter(initPath);
                        classBody += result + (result.Contains(";") ? "" : ";");
                    }
                    // While Loop Handling /////
                    ///////////////////////////
                    if (xReader.Name == "while" && xReader.NodeType == XmlNodeType.Element)
                    {
                        xReader.Read();
                        xReader.Read();
                        var whileReader = xReader.ReadSubtree();
                        string condition = conditionalExpressionReader(whileReader);
                        classBody += $"while({condition})";
                    }
                    //// Do While Loop Handling ///
                    ///////////////////////////
                    if (xReader.Name == "do" && xReader.NodeType == XmlNodeType.Element)
                    {
                        classBody += "do\n";
                    }
                    if (xReader.Name == "for" && xReader.NodeType == XmlNodeType.Element)
                    {
                        xReader.ReadToFollowing("init");
                        /// Obtaining control data on text basis....
                        XmlReader initsubTree = xReader.ReadSubtree();
                        //RulesForCppFile(initsubTree);
                        classBody += DeclarationStatementReaderForCPP(initsubTree);

                        xReader.ReadToFollowing("condition");
                        XmlReader conditionSubree = xReader.ReadSubtree();
                        string condition = conditionalExpressionReader(conditionSubree);

                        xReader.ReadToFollowing("incr");
                        XmlReader incrementPartSubTree = xReader.ReadSubtree();
                        string incr = ExpressionWriter(incrementPartSubTree);
                        //classDefinition += innerText(initPath) + ";";

                        //Writing in ALF Format $ALF RUELS$
                        classBody += "\nwhile(" + condition + ")\n";

                        xReader.ReadToFollowing("block");
                        var block = xReader.ReadSubtree();
                        block.Read();
                        classBody += "{\n";
                        DepthOfBlocks++;
                        RulesForCppFile(block);
                        DepthOfBlocks--;
                        classBody += "\n" + incr + ";\n";
                        classBody += "\n}";
                    }
                    //switch statement
                    //////////////////////
                    if (xReader.Name == "switch")
                    {
                        xReader.ReadToFollowing("condition");
                        var switchTree = xReader.ReadSubtree();
                        string condition = conditionalExpressionReader(switchTree);
                        classBody += "switch (" + condition + ")";
                    }
                    if (xReader.Name == "case")
                    {
                        string caseType = xReader.Name;
                        xReader.ReadToFollowing("expr");
                        string expression = conditionalExpressionReader(xReader.ReadSubtree());
                        classBody += caseType + " " + expression + ":";
                    }
                    if (xReader.Name == "default")
                    {
                        classBody += "default:";
                    }
                }
                // appending white spaces //////
                if (xReader.NodeType == XmlNodeType.Whitespace)
                {
                    if (!IsHeaderCompleted)
                        IncludesAndTopComments += xReader.Value;
                    else
                        classBody += xReader.Value;
                }

                // This one only for while part of do while loop ///
                if (xReader.NodeType == XmlNodeType.Text)
                {
                    if (xReader.Value.Trim() == "while")
                    {
                        xReader.Read();
                        classBody += "while(" + conditionalExpressionReader(xReader.ReadSubtree()) + ");";
                    }
                }
            }
        }

        private void RulesForCustomCode(XmlReader xReader)
        {
            while (xReader.Read())
            {
                if (xReader.NodeType == XmlNodeType.Element)
                {
                    //cpp includes////////////////////
                    //////////////////////////////////
                    if (xReader.Name == "cpp:include")
                    {
                        ReadIncludesAndWriteEquivalnetALF(xReader);
                    }
                    // Declaration Statements Reading
                    /////////////////////////////////////////////////////
                    if (xReader.Name == "decl_stmt")
                    {
                        XmlReader declTree = xReader.ReadSubtree();
                        classBody += DeclarationStatementReaderForCPP(declTree);
                    }

                    //Function Declaration Reading
                    /////////////////////////////////////////////////////////////
                    if (xReader.Name == "function_decl")
                    {
                        bool isVirtual = false;
                        xReader.Read();
                        if (xReader.Name == "specifier")
                        {
                            xReader.Read();
                            if (xReader.Value == "virtual")
                            {
                                isVirtual = true;
                            }
                        }

                        xReader.ReadToFollowing("name");
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        string functionName = xReader.Value;
                        if (!ListOfFunctionsWithVisibility.ContainsKey(functionName))
                            ListOfFunctionsWithVisibility.Add(functionName, new string[] { access_modifier, isVirtual.ToString(), "function" });
                    }
                    // Desctructor Declaration Reading. Needed if there is a virtual destructor.
                    if (xReader.Name == "destructor_decl")
                    {
                        //bool isVirtual = false;
                        //xReader.Read();
                        //if (xReader.Name == "specifier")
                        //{
                        //    xReader.Read();
                        //    if (xReader.Value == "virtual")
                        //    {
                        //        isVirtual = true;
                        //    }
                        //}
                        // At this point, we dont need any further information. We already know destructor Name with an empty body //
                        //ListOfFunctionsWithVisibility.Add(ClassName, new string[] { access_modifier, isVirtual.ToString(), "Destructor" });
                        //xReader.ReadToFollowing("name");
                        //xReader.ReadToFollowing("name");
                        //xReader.Read();
                        //string destructorName = xReader.Value;
                        //ListOfFunctionsWithVisibility.Add(functionName, new string[] { access_modifier, isVirtual.ToString() });
                    }
                    //class declaration Reading ///
                    ////////////////////////
                    if (xReader.Name == "class")
                    {
                        inClassMember = true;
                        IsHeaderCompleted = true;
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        ClassName = xReader.Value;
                        classBody += "class " + ClassName;
                        // clearing old class functions and attributes list // due to occurance of new class.
                        ListOfFunctionsWithVisibility.Clear();
                        classAttributesList.Clear();

                        xReader.Read();
                        xReader.Read();
                        

                    }
                    if (xReader.Name == "super")
                    {
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        classBody += " specializes " + xReader.Value;
                    }

                    // maintaining access modifiers with on going functions /
                    if (xReader.Name == "private")
                    {
                        access_modifier = "private ";
                    }
                    if (xReader.Name == "public")
                    {
                        access_modifier = "public ";
                    }
                    if (xReader.Name == "protected")
                    {
                        access_modifier = "protected ";
                    }

                    //comment//////////////////////////
                    //////////////////////////////////
                    if (xReader.Name == "comment")
                    {
                        CommentReader(xReader);
                    }

                    ////Function Reading////////////////////////
                    /////////////////////////////////////
                    // This also includes coding for main function and other functions // a main function is activity
                    if (xReader.Name == "function")
                    {
                        IsHeaderCompleted = true;
                        xReader.ReadToFollowing("name"); //got type
                        xReader.Read();
                        string funType = xReader.Value;
                        xReader.ReadToFollowing("name"); //got name
                        xReader.Read(); //got value of name
                        string name = xReader.Value;

                        if (name == "main")
                        {
                            classBody += "activity main()";
                            //xReader.ReadToFollowing("block"); //parameters of main skipped at the moment //
                            continue;
                        }

                        xReader.ReadToFollowing("parameter_list");
                        string parametersText = "()";
                        if (xReader.NodeType == XmlNodeType.Element)
                        {
                            XmlReader parameterListInXML = xReader.ReadSubtree();
                            parameterListInXML.Read();
                            parametersText = $"({ParameterRules(parameterListInXML)})";
                        }
                        string alfType = TypeRules.Get(funType).alfName;
                        if (alfType == "")
                            classBody += access_modifier + " " + name + parametersText;
                        else
                            classBody += access_modifier + " " + name + parametersText + " : " + alfType;

                        /// Adding function name in list of functions in class. if this function lies in a class.
                        //ListOfFunctionsWithVisibility.Add(name, new string[] { access_modifier, "False", "function" });

                    }
                    ////expression statement reading ////////////////////////
                    /////////////////////////////////////
                    if (xReader.Name == "expr_stmt")
                    {
                        classBody += ExpressionWriter(xReader.ReadSubtree());
                    }
                    // if and else statement reader
                    //////////////////////////////////////////////////////////
                    if (xReader.Name == "if")
                    {
                        xReader.Read();
                        xReader.Read();
                        var ifReader = xReader.ReadSubtree();
                        string conditionalExpression = conditionalExpressionReader(ifReader);
                        classBody += $"if({conditionalExpression})";
                    }
                    if (xReader.Name == "else")
                    {
                        classBody += "else";
                    }
                    // return statement reading
                    //////////////////////////////////////////
                    if (xReader.Name == "return")
                    {
                        XmlReader subDoc = xReader.ReadSubtree();
                        string text = ExpressionWriter(subDoc);
                        classBody += text;
                    }
                    // function constructor reading
                    ////////////////////////////////////////
                    if (xReader.Name == "constructor")
                    {
                        xReader.ReadToFollowing("operator"); // function name appears after operator
                        xReader.ReadToFollowing("name");
                        xReader.Read();
                        string constructorName = xReader.Value; // getting value

                        xReader.ReadToFollowing("parameter_list");
                        XmlReader subTree = xReader.ReadSubtree();
                        subTree.Read();
                        //xReader.ReadToNextSibling("parameter_list");
                        string parameters = $"({ParameterRules(subTree)})";
                        //Writng constructor in ALF Syntax $ALF Rules For Constructor$
                        classBody += "\n@Create\npublic " + constructorName + parameters;
                        inConstructor = true;
                    }
                    //// Construction Initializers , 
                    /////////////////////////////
                    if (xReader.Name == "member_init_list")
                    {
                        XmlReader members_initsSubTree = xReader.ReadSubtree();
                        xReader.Read();
                        ReadConstructorInitializers(members_initsSubTree);
                    }
                    /// The Block Reader
                    /// //////////////////
                    if (xReader.Name == "block")
                    {
                        IsHeaderCompleted = true;
                        var block = xReader.ReadSubtree();
                        block.Read();
                        classBody += "{\n";
                        if (inConstructor) //writing constructor
                        {
                            classBody += constructorInit + "\n";
                            constructorInit = "";
                            inConstructor = false;
                        }
                        DepthOfBlocks++;
                        RulesForCustomCode(block);
                        DepthOfBlocks--;
                        classBody += "\n}";

                        if (DepthOfBlocks == 0)
                        {
                            classAttributesList.Clear();
                            ListOfFunctionsWithVisibility.Clear();
                            inClassMember = false;
                        }
                    }

                    // Enum Handling //
                    ///////////////////////////
                    if (xReader.Name == "enum")
                    {
                        //enum TBD ///
                        XmlReader enumTree = xReader.ReadSubtree();
                        while (enumTree.Read()) ;
                    }

                    // Initialization things
                    ///////////////////////////////////
                    if (xReader.Name == "init")
                    {
                        XmlReader initPath = xReader.ReadSubtree();
                        string result = ExpressionWriter(initPath);
                        classBody += result + (result.Contains(";") ? "" : ";\n");
                    }
                    // While Loop Handling /////
                    ///////////////////////////
                    if (xReader.Name == "while" && xReader.NodeType == XmlNodeType.Element)
                    {
                        xReader.Read();
                        xReader.Read();
                        var whileReader = xReader.ReadSubtree();
                        string condition = conditionalExpressionReader(whileReader);
                        classBody += $"while({condition})";
                    }
                    //// Do While Loop Handling ///
                    ///////////////////////////
                    if (xReader.Name == "do" && xReader.NodeType == XmlNodeType.Element)
                    {
                        classBody += "do\n";
                    }
                    if (xReader.Name == "for" && xReader.NodeType == XmlNodeType.Element)
                    {
                        xReader.ReadToFollowing("init");
                        /// Obtaining control data on text basis....
                        XmlReader initsubTree = xReader.ReadSubtree();
                        //RulesForCustomCode(initsubTree);
                        classBody += DeclarationStatementReaderForCPP(initsubTree);

                        xReader.ReadToFollowing("condition");
                        XmlReader conditionSubree = xReader.ReadSubtree();
                        string condition = conditionalExpressionReader(conditionSubree);

                        xReader.ReadToFollowing("incr");
                        XmlReader incrementPartSubTree = xReader.ReadSubtree();
                        string incr = ExpressionWriter(incrementPartSubTree);
                        //classDefinition += innerText(initPath) + ";";

                        //Writing in ALF Format $ALF RUELS$
                        classBody += "\nwhile(" + condition + ")\n";

                        xReader.ReadToFollowing("block");
                        var block = xReader.ReadSubtree();
                        block.Read();
                        classBody += "{\n";
                        DepthOfBlocks++;
                        RulesForCustomCode(block);
                        DepthOfBlocks--;
                        classBody += "\n" + incr + ";\n";
                        classBody += "\n}";
                    }
                    //switch statement
                    //////////////////////
                    if (xReader.Name == "switch")
                    {
                        xReader.ReadToFollowing("condition");
                        var switchTree = xReader.ReadSubtree();
                        string condition = conditionalExpressionReader(switchTree);
                        classBody += "switch (" + condition + ")";
                    }
                    if (xReader.Name == "case")
                    {
                        string caseType = xReader.Name;
                        xReader.ReadToFollowing("expr");
                        string expression = conditionalExpressionReader(xReader.ReadSubtree());
                        classBody += caseType + " " + expression + ":";
                    }
                    if (xReader.Name == "default")
                    {
                        classBody += "default:";
                    }
                }
                // appending white spaces //////
                if (xReader.NodeType == XmlNodeType.Whitespace)
                {
                    if (!IsHeaderCompleted)
                        IncludesAndTopComments += xReader.Value;
                    else
                        classBody += xReader.Value;
                }

                // This one only for while part of do while loop ///
                if (xReader.NodeType == XmlNodeType.Text)
                {
                    if (xReader.Value.Trim() == "while")
                    {
                        xReader.Read();
                        classBody += "while(" + conditionalExpressionReader(xReader.ReadSubtree()) + ");";
                    }
                }
            }
        }

        private string ParameterRules(XmlReader xReader)
        {
            string parameterText = "";
            while (xReader.Read())
            {
                if (xReader.NodeType == XmlNodeType.Element)
                {
                    xReader.Read();
                    if (xReader.Name == "decl")
                    {
                        xReader.ReadToFollowing("name");
                        xReader.Read(); //got type
                        string type = xReader.Value;
                        xReader.ReadToFollowing("name");
                        xReader.Read(); // got name
                        string name = xReader.Value;
                        //Writing parameters in ALF Syntax $ALF Parameter Rules$
                        parameterText += "in " + name + " : " + TypeRules.Get(type).alfName;
                    }
                }
                if (xReader.NodeType == XmlNodeType.Text && xReader.Value.Contains(","))
                    parameterText += xReader.Value;
            }
            return parameterText;
        }

        private void CommentReader(XmlReader xReader)
        {
            string type = xReader.GetAttribute("type");
            xReader.Read();
            string value = xReader.Value;
            if (!IsHeaderCompleted)
                IncludesAndTopComments += value;
            else
                classBody += value;
        }

        private string DeclarationStatementReaderForCPP(XmlReader xReader)
        {
            string result = "";
            string tempType = "";
            while (xReader.ReadToFollowing("decl"))
            {
                var oneDeclaration = xReader.ReadSubtree();
                oneDeclaration.Read();
                if (oneDeclaration.NodeType == XmlNodeType.Element)
                {
                    if (tempType == "")
                    {
                        oneDeclaration.ReadToFollowing("name");
                        oneDeclaration.Read(); //got type
                        string type = oneDeclaration.Value;
                        tempType = type;
                    }
                    oneDeclaration.ReadToFollowing("name");
                    oneDeclaration.Read(); // got name
                    string name = oneDeclaration.Value;
                    if (name.Trim() == "") // this if blocks is used for getting inconsistent var name in srcML
                    {
                        oneDeclaration.Read();
                        name = oneDeclaration.Value;
                    }

                    if (DepthOfBlocks == 1 && inClassMember) //if it is in a class
                    {
                        result += access_modifier + name + " : " + TypeRules.Get(tempType).alfName;
                        classAttributesList.Add(name);
                    }
                    else
                    {
                        result += TypeRules.Get(tempType).alfName + " " + name; //if new will be set here if needed
                    }

                    if (!oneDeclaration.ReadToFollowing("init"))
                        result += ";\n";
                    else
                        result += ExpressionWriter(oneDeclaration.ReadSubtree()) + ";\n";
                }
            }
            return result;
        }

        private string conditionalExpressionReader(XmlReader xReader)
        {
            string condition = "";
            while (xReader.Read())
            {
                if (xReader.NodeType == XmlNodeType.Element)
                {
                    if (xReader.Name == "expr")
                    {
                        XmlReader exprReader = xReader.ReadSubtree();
                        string text = ExpressionWriter(exprReader);
                        condition += text;
                    }
                }
            }

            return condition;
        }

        private string ExpressionWriter(XmlReader xReader)
        {
            string text = "";
            bool firstThis = true;
            bool lastBracket = false;
            bool firstCoutOprPassed = false;
            while (xReader.Read())
            {
                if (xReader.NodeType == XmlNodeType.Text)
                {
                    string val = xReader.Value;
                    if (firstThis && (classAttributesList.Contains(val) || ListOfFunctionsWithVisibility.ContainsKey(val)))
                    {
                        val = "this." + val;
                        firstThis = false;
                    }
                    else if (val == "cout")
                    {
                        lastBracket = true;
                        firstCoutOprPassed = false;
                        val = "WriteLine(";
                    }
                    else if (val == "cin")
                    {
                        lastBracket = true;
                        val = "ReadLine(";
                    }
                    if (val == ",")
                        firstThis = true;

                    text += val;
                }
                if (xReader.Name == "operator" && xReader.NodeType == XmlNodeType.Element)
                {
                    xReader.Read();
                    string opr = xReader.Value;
                    if (opr == "->")
                    {
                        text += ".";
                    }
                    else if (opr == "<<")
                    {
                        if (firstCoutOprPassed) // because: cout<<var1<<"this is another"<<var2;
                        {
                            opr = "+";
                            text += opr;
                        }
                        firstCoutOprPassed = true;
                        // do nothing // skipping these operators for cpp //
                    }
                    else if (opr == ">>")
                    {
                        // do nothing // skipping these operators for cpp //
                    }
                    else if (opr != ".")
                    {
                        firstThis = true;
                        text += " " + opr + " ";
                    }
                    else
                        text += opr;

                }
                // remove it 
                if (xReader.NodeType == XmlNodeType.Whitespace)
                {
                    text += xReader.Value;
                }
            }
            if (lastBracket)
            {
                if (text.Contains(";"))
                {
                    text = text.Replace(";", ");");
                }
            }
            return text;
        }

        private void ReadIncludesAndWriteEquivalnetALF(XmlReader xReader)
        {
            bool b = xReader.ReadToFollowing("cpp:file");
            xReader.Read();
            string value = xReader.Value.Trim(new char[] { '"', '<', '>' });
            value = Path.GetFileNameWithoutExtension(value);
            if ((TypeRules.Contains(value, true) && value != ClassName) || Custom)
                IncludesAndTopComments += "private import DefaultPkg::" + value;// + ";\n";
        }

        private void ReadConstructorInitializers(XmlReader xReader)
        {
            while (xReader.ReadToFollowing("call"))
            {
                xReader.ReadToFollowing("name");
                xReader.Read();
                string leftSideOfExpression = xReader.Value;
                if (classAttributes.Contains(leftSideOfExpression))
                    leftSideOfExpression = "this." + leftSideOfExpression;
                xReader.ReadToFollowing("argument");
                XmlReader argTree = xReader.ReadSubtree();
                string rightSideOfExpression = ExpressionWriter(argTree); // innerText itsleft provides this.

                string result = leftSideOfExpression + " = " + rightSideOfExpression + ";\n";
                constructorInit += result;
            }
        }
        #endregion

    }
}
