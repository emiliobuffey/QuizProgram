using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class QuizProgram
{
    static void Main()
    {
        Console.WriteLine("Welcome to the Quiz!");
        string? yesOrNo = "Y";
        while(yesOrNo == "Y" || yesOrNo == "y" || yesOrNo == "Yes" || yesOrNo == "yes")
        {
            // Would you like to take a quiz?
            Console.WriteLine("\nEnter: Y, y, Yes, or yes to take a quiz. \nEnter: anything else to exit.");
            yesOrNo = Console.ReadLine();

            if (!(yesOrNo == "Y" || yesOrNo == "y" || yesOrNo == "Yes" || yesOrNo == "yes"))
            {
                return; // Break the loop if input is not Y, y, Yes, or yes
            }

            // Method to read questions from the file
            List<Question> questions = ReadQuestions();

            // Check if questions were successfully loaded
            if (questions.Count == 0 )
            {
                Console.WriteLine("Quiz cannot be started without questions.");
                Console.WriteLine("Please make sure the quiz file has the appropriate formatting and tags (@Q,@A,@E).\n");
                return;
            }

            // Get the number of questions to ask
            Console.Write("\nEnter the number of questions to ask: ");
            if (!int.TryParse(Console.ReadLine(), out int numQuestions) || numQuestions <= 0)
            {
                Console.WriteLine("Invalid input for the number of questions. Exiting quiz.");
                return;
            }

            // Start the quiz
            QuizSession(questions, numQuestions);
        }
    }

    static List<Question> ReadQuestions()
    {
        List<Question> questions = new List<Question>();
        // Keep trying to read from the file path until a valid one is entered
        while (true)
        {
            Console.Write("\nEnter the full path of the quiz questions file: ");
            string? filePath = Console.ReadLine();

            if(!string.IsNullOrEmpty(filePath))
            {
                try
                { 
                    string[] lines = File.ReadAllLines(filePath);

                    // Loop through ever line in the file one at a time
                    for (int i = 0; i < lines.Length; i++)
                    {
                        // Ignore new lines and lines starting with *
                        if (lines[i].StartsWith("*") || string.IsNullOrWhiteSpace(lines[i]))
                        {
                            continue;
                        }

                        if (lines[i].StartsWith("@Q"))
                        {
                            string questionText = string.Empty;
                            List<string> answers = new List<string>();
                            int answer = 0;

                            while (!lines[i].StartsWith("@A"))
                            {
                                if(!lines[i].StartsWith("@Q"))
                                {
                                    // store the question 
                                    questionText += lines[i] + " ";
                                }
                                i++;
                            }

                            if (lines[i].StartsWith("@A"))
                            {
                                i++;
                                if (int.TryParse(lines[i], out int ans))
                                {
                                    // store the correct answer
                                    answer = ans;
                                }
                                else
                                {
                                    Console.WriteLine("The answer in the file was not an integer! I set the correct answer to zero to handle the invalid input.");
                                    answer = 0;
                                }

                                while (!lines[i].StartsWith("@E"))
                                {
                                    // Store the answers
                                    answers.Add(lines[i]);
                                    i++;
                                }
                            }

                            // Store the question, possible answers, and the correct answer
                            questions.Add(new Question { Text = questionText, Answers = answers, Answer = answer });
                        }
                    }

                    // If no exception occurred, break out of the loop
                    break;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"File not found: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading questions: {ex.Message}");
                }
            }
        }
        return questions;
    }

    static void QuizSession(List<Question> questions, int numQuestions)
    {
        Random random = new Random();
        HashSet<int> askedQuestionIndices = new HashSet<int>();
        int correctAnswers = 0;

        // Start the stopwatch
        Stopwatch stopwatch = Stopwatch.StartNew();


        for (int i = 0; i < numQuestions && askedQuestionIndices.Count < questions.Count; i++)
        {
            int randomIndex;

            // Get a random question
            do
            {
                randomIndex = random.Next(questions.Count);
            } while (askedQuestionIndices.Contains(randomIndex));

            askedQuestionIndices.Add(randomIndex);

            Question currentQuestion = questions[randomIndex];

            // Display the question
            Console.WriteLine($"\nQuestion: {currentQuestion.Text}");

            // Display the answers with a number attached to the from of them
            for (int j = 1; j < currentQuestion.Answers.Count; j++)
            {
                Console.WriteLine($"{j}. {currentQuestion.Answers[j]}");
            }

            // Get the users input and check if correct
            int userAnswer;
            if (int.TryParse(Console.ReadLine(), out userAnswer) && userAnswer >= 1 && userAnswer <= currentQuestion.Answers.Count)
            {

                if (userAnswer == currentQuestion.Answer)
                {
                    Console.WriteLine("Correct!");
                    correctAnswers++;
                }
                else
                {
                    Console.WriteLine($"Wrong! The correct answer was: {currentQuestion.Answers[0]}");
                }
            }
            else
            {
                Console.WriteLine($"Invalid input. Skipping question. The correct answer was: {currentQuestion.Answers[0]}");
            }
        }

        // Stop the stopwatch
        stopwatch.Stop();

        // Display quiz summary including elapsed time
        Console.WriteLine("\n-------------------------------------------------------------------------------\nQuiz Summary:");
        Console.WriteLine($"Total questions asked: {askedQuestionIndices.Count}");
        Console.WriteLine($"Total correct answers: {correctAnswers}");
        Console.WriteLine($"Percentage correct: {((double)correctAnswers / askedQuestionIndices.Count) * 100:F2}%");
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}\n-------------------------------------------------------------------------------\n");
    }
}

class Question
{
    public string Text { get; set; } = string.Empty;
    public List<string> Answers { get; set; } = new();
    public int Answer { get; set; }
}
