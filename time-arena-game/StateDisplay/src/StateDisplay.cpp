#include <CanvasTriangle.h>
#include <DrawingWindow.h>
#include <Utils.h>
#include <iostream>
#include <fstream>
#include <unordered_map>
#include <vector>

using namespace std;

#define STATE_LOG "stateLog.txt"
#define TIME_LOG "timeLog.txt"

void parse(string s, string del, vector<string> &v)
{
    int start = 0;
    int end = s.find(del);
    while (end != -1) {
        v.push_back(s.substr(start, end - start));
        start = end + del.size();
        end = s.find(del, start);
    }
    v.push_back(s.substr(start, end - start));
}

ostream &operator<<(ostream &os, const vector<string> &input)
{
    for (auto const &i: input) {
        os << i << " ";
    }
    return os;
}

uint32_t generateRandomColour() {
    float red = rand() % 256;
    float green = rand() % 256;
    float blue = rand() % 256;
    uint32_t colour = (255 << 24) + (int(red) << 16) + (int(green) << 8) + int(blue);
    return colour;
}

// Returns true if the given colour is too similar to an existing assigned colour.
bool isTooSimilar(uint32_t colour, unordered_map<string, uint32_t> &tails) {
    int red = (colour >> 16) & 0xff;
    int green = (colour >> 8) & 0xff;
    int blue = colour & 0xff;
    for (const auto &tail: tails) {
        int tailRed = (tail.second >> 16) & 0xff;
        int tailGreen = (tail.second >> 8) & 0xff;
        int tailBlue = tail.second & 0xff;
        if (abs(red - tailRed) < 10 && abs(green - tailGreen) < 10 && abs(blue - tailBlue) < 10) return true;
    }
    return false;
}

// Returns the assigned colour of the given tail and assigns a new colour if not yet assigned.
uint32_t getColour(string tailID, unordered_map<string, uint32_t> &tails) {
    // Return the colour if already assigned.
    auto tail = tails.find(tailID);
    if (tail != tails.end()) {
        return tail->second;
    }

    // Generate a new colour.
    uint32_t colour = generateRandomColour();
    while (isTooSimilar(colour, tails)) {
        colour = generateRandomColour();
    }
    tails.insert({tailID, colour});
    return colour;
}

void drawStateLog(DrawingWindow &window, ifstream &stateLog, int totalFrames, unordered_map<string, uint32_t> &tails) {
    string line;
    vector<string> tailIDs{};
    int x;
    int y;
    string tailID;

    while (getline(stateLog, line)) {
        parse(line, "-", tailIDs);

        if (tailIDs.size() > 1) {
            cout << tailIDs[0] << endl;
            x = stoi(tailIDs[0]);
            for (int i=1; i < tailIDs.size(); i++) {
                tailID = tailIDs[i];
                uint32_t colour = getColour(tailID, tails);
                y = 50 - i;
                if (x >= 0 && x < totalFrames && y >= 0 && y < totalFrames) {
                    window.setPixelColour(x, y, colour);
                }
            }
        }

        line = "";
        tailIDs = {};
    }
}

void drawTimeLog(DrawingWindow &window, ifstream &timeLog, int totalFrames, unordered_map<string, uint32_t> &tails) {
    string line;
    vector<string> objects;
    vector<string> objectData;
    int x;
    int y = 50;
    uint32_t white = 0xFFFFFF;
    uint32_t colour;

    while (getline(timeLog, line)) {
        parse(line, ",", objects);
        for (auto object: objects) {
            parse(object, "-", objectData);
            cout << objectData[2] << endl;
            x = stoi(objectData[2]);
            if (objectData[0] == "p") {
                window.setPixelColour(x, y, white);
            }
            else {
                colour = getColour(objectData[1], tails);
                window.setPixelColour(x, y, colour);
            }
            objectData = {};
        }
        objects = {};
        y++;
    }
}


int createWindow(DrawingWindow &window, ifstream &timeLog) {
    string firstLine;
    getline(timeLog, firstLine);
    cout << firstLine << endl;
    int totalFrames = stoi(firstLine);
    window = DrawingWindow(totalFrames, totalFrames + 50, false);
    return totalFrames;
}

int main(int argc, char *argv[]) {
    DrawingWindow window;

    unordered_map<string, uint32_t> tails;

    string stateLogFilename = STATE_LOG;
    string timeLogFilename = TIME_LOG;

    ifstream timeLog(timeLogFilename);
    ifstream stateLog(stateLogFilename);

    if (timeLog.is_open()) {
        if (stateLog.is_open()) {
            int totalFrames = createWindow(window, timeLog);
            window.clearPixels();
            drawTimeLog(window, timeLog, totalFrames, tails);
            drawStateLog(window, stateLog, totalFrames, tails);
            window.savePPM("output.ppm");
            window.saveBMP("output.bmp");
            timeLog.close();
            stateLog.close();
        }
        else cout << "Can't open state log file" << endl;
    }
    else cout << "Can't open time log file" << endl;
}
