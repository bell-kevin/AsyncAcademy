from selenium import webdriver
from selenium.webdriver.common.by import By
#from selenium.webdriver.support.wait import WebDriverWait
from selenium.common.exceptions import NoSuchElementException, ElementNotInteractableException
import time
import random

WEBSITE_URL = "https://asyncacademy20241005173644.azurewebsites.net/"
driver = webdriver.Edge()
#errors = [NoSuchElementException, ElementNotInteractableException]
#wait = WebDriverWait(driver, 10, poll_frequency=0.5, ignored_exceptions=errors)

# Helper method that will attempt to locate an element, 
# if it fails it will wait one second and try again until
# maximum number of retries is reached
def find_element(by, value, retries=10):
    for i in range(retries):
        try:
            element = driver.find_element(by, value)
            driver.execute_script("arguments[0].scrollIntoView();", element)
            time.sleep(1)
            return element
        except Exception as e:
            time.sleep(1)
    try:
        raise e
    except UnboundLocalError:
        raise Exception

def testLogIn(): # Make sure logging in with test users works
    explanation = ""
    try:
        # Load website
        explanation = "Failed to load website, are you connected to the internet? Is the website up? Did its URL change?"
        driver.get(WEBSITE_URL)
        # try logging in with both test accounts
        for username in ("studenttest","instructortest"):
            # Find username and password fields
            explanation = "Failed to find username or password field, did those change?"
            username_field = find_element(By.NAME, "Account.Username")
            password_field = find_element(By.NAME, "Account.Pass")
            explanation = "Failed to find login button, did its HTML change?"
            login_button = find_element(By.XPATH, "/html/body/div/main/div/form/div[3]/div/input")
            explanation = "Failed to log in as test student, did the login info for the test student change?"
            time.sleep(1)
            username_field.send_keys(username)
            password_field.send_keys("Pass1234")
            login_button.click()
            time.sleep(5)
            explanation = "Either the website failed to load, or the url for the welcome page simply changed, in which case this test needs to be updated with the new url"
            assert driver.current_url == "https://asyncacademy20241005173644.azurewebsites.net/welcome"
            driver.get(WEBSITE_URL)
            time.sleep(5)
        return True, None, ""
    except Exception as e:
        return False, e, explanation

def testAccountCreation():
    explanation = ""
    try:
        # Load website
        explanation = "Failed to load website, are you connected to the internet? Is the website up? Did its URL change?"
        driver.get(WEBSITE_URL)
        # Click sign up button
        explanation = "Failed to find or interact with an element"
        sign_up_button = find_element(By.XPATH, "/html/body/div/main/div/form/div[3]/div/a/button")
        sign_up_button.click()
        username_field = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[1]/input")
        first_name_field = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[2]/input")
        last_name_field = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[3]/input")
        email_field = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[4]/input")
        password_field = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[5]/input")
        confirm_password_field = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[6]/input")
        birthday_field = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[7]/input[1]")
        # Input data
        test_username = []
        for i in range(50):
            test_username.append(chr(random.randint(65,122)))
        test_username = ''.join(test_username)
        username_field.send_keys(test_username)
        first_name_field.send_keys("Selenium")
        last_name_field.send_keys("Test")
        email_field.send_keys("seleniumtest@gmail.com")
        password_field.send_keys("Pass1234")
        confirm_password_field.send_keys("Pass1234")
        birthday_field.click()
        birthday_field.send_keys("03112006") # TODO: Test that birthdates too young don't work
        submit_button = find_element(By.XPATH, "/html/body/div/main/div/div/form/div[9]/input")
        submit_button.click()
        time.sleep(5)
        explanation = "Got unexpected URL"
        assert driver.current_url == "https://asyncacademy20241005173644.azurewebsites.net/welcome"
        explanation = "Was unable to find or interact with element at login page"
        driver.get(WEBSITE_URL)
        username_field = find_element(By.NAME, "Account.Username")
        password_field = find_element(By.NAME, "Account.Pass")
        login_button = find_element(By.XPATH, "/html/body/div/main/div/form/div[3]/div/input")
        username_field.send_keys(test_username)
        password_field.send_keys("Pass1234")
        login_button.click()
        time.sleep(5)
        explanation = "Got unexpected URL"
        assert driver.current_url == "https://asyncacademy20241005173644.azurewebsites.net/welcome"
        return True, None, ""
    except Exception as e:
        return False, e, explanation

def testGraphVisibility():
    explanation = ""
    try:
        # Student
        explanation = "Failed to load website, are you connected to the internet? Is the website up? Did its URL change?"
        driver.get(WEBSITE_URL)
        explanation = "Failed to log in as test student"
        username_field = find_element(By.NAME, "Account.Username")
        password_field = find_element(By.NAME, "Account.Pass")
        login_button = find_element(By.XPATH, "/html/body/div/main/div/form/div[3]/div/input")
        username_field.send_keys("studenttest")
        password_field.send_keys("Pass1234")
        login_button.click()
        time.sleep(5)
        assert driver.current_url == "https://asyncacademy20241005173644.azurewebsites.net/welcome"
        explanation = "Unable to locate CS 3550 link"
        cs3550_link = find_element(By.LINK_TEXT, "CS 3550")
        explanation = "Unable to interact with CS 3550 link"
        time.sleep(3)
        driver.execute_script("arguments[0].scrollIntoView();", cs3550_link) # scroll element into view
        time.sleep(3)
        cs3550_link.click()
        time.sleep(5)
        explanation = "Failed to enter CS 3550 class card, got unexpected URL"
        assert "https://asyncacademy20241005173644.azurewebsites.net/ClassOverview?" in driver.current_url
        try:
            find_element(By.XPATH, "/html/body/div/main/div[2]", 1)
            raise Exception("Found graph when it shouldn't have been there")
        except:
            pass
         # Instructor
        explanation = "Failed to load website, are you connected to the internet? Is the website up? Did its URL change?"
        driver.get(WEBSITE_URL)
        explanation = "Failed to log in as test student"
        username_field = find_element(By.NAME, "Account.Username") 
        password_field = find_element(By.NAME, "Account.Pass")
        login_button = find_element(By.XPATH, "/html/body/div/main/div/form/div[3]/div/input")
        username_field.send_keys("instructortest")
        password_field.send_keys("Pass1234")
        login_button.click()
        time.sleep(5)
        assert driver.current_url == "https://asyncacademy20241005173644.azurewebsites.net/welcome"
        explanation = "Unable to locate CS 3550 link"
        cs3550_link = find_element(By.LINK_TEXT, "CS 3550")
        explanation = "Unable to interact with CS 3550 link"
        time.sleep(3)
        driver.execute_script("arguments[0].scrollIntoView();", cs3550_link) # scroll element into view
        time.sleep(3)
        cs3550_link.click()
        time.sleep(5)
        explanation = "Got unexpected URL"
        #assert "https://asyncacademy20241005173644.azurewebsites.net/ClassOverview?" in driver.current_url
        explanation = "Failed to find graph"
        find_element(By.XPATH, "/html/body/div/main/div[2]", 5)
        return True, None, ""
    except Exception as e:
        return False, e, explanation